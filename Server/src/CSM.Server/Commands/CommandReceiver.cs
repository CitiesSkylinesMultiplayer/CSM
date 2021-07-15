using CSM.Commands.Data.Game;
using CSM.Commands.Data.Internal;
using CSM.Commands.Handler;
using CSM.Commands.Handler.Internal;
using CSM.Helpers;
using CSM.Networking;
using CSM.Server.Util;
using LiteNetLib;
using System;
using System.IO;

namespace CSM.Commands
{
    public static class CommandReceiver
    {
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

            if (cmd == null)
            {
                return true;
            }

            if (handler == null)
            {
                return true;
            }

            // Handle connection request as special case
            if (cmd.GetType() == typeof(ConnectionRequestCommand))
            {
                ((ConnectionRequestHandler)handler).HandleOnServer((ConnectionRequestCommand)cmd, peer);
                return false;
            }

            // Intercept response command for the time
            if(cmd is SpeedPauseResponseCommand speedPauseResponseCommand)
            {
                SpeedPauseHelper.SetCurrentTime(speedPauseResponseCommand.CurrentTime);
            }

            // Make sure we know about the connected client on the server
            if (!MultiplayerManager.Instance.CurrentServer.ConnectedPlayers.ContainsKey(peer.Id))
            {
                Log.Warn("Client tried to send packet but never joined with a ConnectionRequestCommand. Ignoring...");
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

            if (cmd == null)
            {
                handler = null;
                cmd = null;
                return;
            }

            Log.Debug($"Received {cmd.GetType().Name}");

            handler = Command.GetCommandHandler(cmd.GetType());
            if (handler == null)
            {
                Log.Error($"Command Handler {cmd.GetType().Name} not found!");
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
            try
            {
                CommandBase result;

                using (MemoryStream stream = new MemoryStream(message))
                {
                    result = (CommandBase)Command.Model.Deserialize(stream, null, typeof(CommandBase));
                }

                return result;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
