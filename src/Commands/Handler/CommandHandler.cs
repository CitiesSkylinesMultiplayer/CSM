using CSM.Networking;
using System;

namespace CSM.Commands.Handler
{
    public abstract class CommandHandler
    {
        /// <summary>
        ///     If this is true, client -> server packets are relayed to all other clients.
        /// </summary>
        public bool RelayOnServer { get; protected set; } = true;

        /// <summary>
        ///     If this is true, this command is only executed after the FinishTransactionCommand is received.
        /// </summary>
        public bool TransactionCmd { get; protected set; } = true;

        public abstract Type GetDataType();

        public abstract void Parse(CommandBase message);

        public virtual void OnClientConnect(Player player)
        {
        }

        public virtual void OnClientDisconnect(Player player)
        {
        }
    }

    public abstract class CommandHandler<C> : CommandHandler where C : CommandBase
    {
        public abstract void Handle(C command);

        public override Type GetDataType()
        {
            return typeof(C);
        }

        public override void Parse(CommandBase command)
        {
            Handle((C)command);
        }
    }
}
