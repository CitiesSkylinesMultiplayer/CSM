using CSM.Commands.Handler;
using CSM.Networking;
using LiteNetLib;
using System.IO;

namespace CSM.Commands
{
    public static class CommandReceiver
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     This method is used to parse an incoming message on the client
        ///     and execute the appropriate actions.
        /// </summary>
        /// <param name="reader">The incoming packet including the command type byte.</param>
        /// <param name="peer">The peer object of the sending client.</param>
        /// <returns>If the command should be forwarded to other clients.</returns>
        public static bool Parse(NetPacketReader reader, NetPeer peer)
        {
            Parse(reader, out CommandHandler handler, out CommandBase cmd);

            if (handler == null)
            {
                return false;
            }

            // Handle connection request as special case
            if (cmd.GetType() == typeof(ConnectionRequestCommand))
            {
                ((ConnectionRequestHandler)handler).HandleOnServer((ConnectionRequestCommand)cmd, peer);
                return false;
            }

            // Make sure we know about the connected client on the server
            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server && !MultiplayerManager.Instance.CurrentServer.ConnectedPlayers.ContainsKey(peer.Id))
            {
                _logger.Warn("Client tried to send packet but never joined with a ConnectionRequestCommand. Ignoring...");
                return false;
            }

            if (TransactionHandler.CheckReceived(handler, cmd))
            {
                return handler.RelayOnServer;
            }

            handler.Parse(cmd);

            return handler.RelayOnServer;
        }

        /// <summary>
        ///     This method is used to extract the command type from an incoming message
        ///     and return the matching handler object.
        /// </summary>
        /// <param name="reader">The incoming packet including the command type byte.</param>
        /// <param name="handler">This returns the command handler object. May be null if the command was not found.</param>
        /// <param name="cmd">This returns the command data object.</param>
        private static void Parse(NetPacketReader reader, out CommandHandler handler, out CommandBase cmd)
        {
            cmd = Deserialize(reader.GetRemainingBytes());

            _logger.Info($"Received {cmd.GetType().Name}");

            handler = Command.GetCommandHandler(cmd.GetType());
            if (handler == null)
            {
                _logger.Error($"Command {cmd.GetType().Name} not found!");
                return;
            }
        }

        /// <summary>
        ///     Deserialize the command from a byte array.
        /// </summary>
        /// <param name="message">A byte array of the message</param>
        /// <returns>The deserialized command.</returns>
        private static CommandBase Deserialize(byte[] message)
        {
            CommandBase result;

            using (MemoryStream stream = new MemoryStream(message))
            {
                result = (CommandBase)Command.Model.Deserialize(stream, null, typeof(CommandBase));
            }

            return result;
        }
    }
}
