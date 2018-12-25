using CSM.Networking;
using ProtoBuf;
using System;
using System.IO;

namespace CSM.Commands.Handler
{
    public abstract class CommandHandler
    {
        public abstract byte ID { get; }

        /// <summary>
        /// If this is true, client -> server packets are relayed to all other clients.
        /// </summary>
        public bool RelayOnServer { get; protected set; } = true;

        /// <summary>
        /// If this is not NONE, all packets of this TransactionType are queued until a FinishTransactionCommand is received.
        /// </summary>
        public TransactionType Transaction { get; protected set; } = TransactionType.NONE;

        public abstract Type GetDataType();

        public abstract void ParseOnServer(byte[] message, Player player);

        public abstract void ParseOnClient(byte[] message);

        public virtual void OnClientConnect(Player player)
        {
        }

        public virtual void OnClientDisconnect(Player player)
        {
        }
    }

    public abstract class CommandHandler<C> : CommandHandler where C : CommandBase
    {
        public abstract void HandleOnServer(C command, Player player);

        public abstract void HandleOnClient(C command);

        public override Type GetDataType()
        {
            return typeof(C);
        }

        public override void ParseOnServer(byte[] message, Player player)
        {
            HandleOnServer(Parse(message), player);
        }

        public override void ParseOnClient(byte[] message)
        {
            HandleOnClient(Parse(message));
        }

        /// <summary>
        ///     Deserialize the command from a byte array.
        /// </summary>
        /// <param name="message">A byte array of the message</param>
        /// <returns>The deserialized command.</returns>
        protected C Parse(byte[] message)
        {
            C result;

            using (var stream = new MemoryStream(message))
            {
                result = Serializer.Deserialize<C>(stream);
            }

            return result;
        }
    }
}