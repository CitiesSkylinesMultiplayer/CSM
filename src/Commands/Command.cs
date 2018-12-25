using CSM.Commands.Handler;
using CSM.Networking;
using CSM.Networking.Status;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSM.Commands
{
    public class Command
    {
        private static readonly Dictionary<byte, CommandHandler> _handlerMapping = new Dictionary<byte, CommandHandler>();
        private static readonly Dictionary<Type, byte> _cmdMapping = new Dictionary<Type, byte>();

        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// This method is used to parse an incoming message on the client
        /// and execute the appropriate actions.
        /// </summary>
        /// <param name="reader">The incoming packet including the command type byte.</param>
        public static void ParseOnClient(NetPacketReader reader)
        {
            Parse(reader, out CommandHandler handler, out byte[] message);

            if (handler == null)
                return;

            if (TransactionHandler.CheckReceived(handler, message, null))
            {
                return;
            }

            handler.ParseOnClient(message);
        }

        /// <summary>
        /// This method is used parse an incoming message on the server
        /// and execute the appropriate actions.
        /// </summary>
        /// <param name="reader">The incoming packet including the command type byte.</param>
        /// <param name="player">The player object of the sending client. May be null if the sender is not known.</param>
        public static bool ParseOnServer(NetPacketReader reader, Player player)
        {
            Parse(reader, out CommandHandler handler, out byte[] message);

            if (handler == null)
                return false;

            // Make sure we know about the connected client
            if (player == null)
            {
                _logger.Warn($"Client tried to send packet {handler.GetType().Name} but never joined with a ConnectionRequestCommand packet. Ignoring...");
                return false;
            }

            if (TransactionHandler.CheckReceived(handler, message, player))
            {
                return handler.RelayOnServer;
            }

            handler.ParseOnServer(message, player);

            return handler.RelayOnServer;
        }

        /// <summary>
        /// This method is used to extract the command type from an incoming message
        /// and return the matching handler object.
        /// </summary>
        /// <param name="reader">The incoming packet including the command type byte.</param>
        /// <param name="handler">This returns the command handler object. May be null if the command was not found.</param>
        /// <param name="message">This returns the message byte array without the command type byte.</param>
        public static void Parse(NetPacketReader reader, out CommandHandler handler, out byte[] message)
        {
            // The message type is the first byte, (255 message types)
            byte messageType = reader.GetByte();

            // Skip the first byte
            message = reader.GetRemainingBytes();

            if (!_handlerMapping.TryGetValue(messageType, out handler))
            {
                _logger.Error($"Command {messageType} not found!");
                return;
            }
        }

        /// <summary>
        /// This method is used to send a command to a connected client.
        /// Does only work if the current game acts as a server.
        /// </summary>
        /// <param name="peer">The NetPeer to send the command to.</param>
        /// <param name="command">The command to send.</param>
        public static void SendToClient(NetPeer peer, CommandBase command)
        {
            if (MultiplayerManager.Instance.CurrentRole != MultiplayerRole.Server)
                return;

            byte id = _cmdMapping[command.GetType()];
            MultiplayerManager.Instance.CurrentServer.SendToClient(peer, id, command);
        }

        /// <summary>
        /// This method is used to send a command to a connected client.
        /// Does only work if the current game acts as a server.
        /// </summary>
        /// <param name="player">The Player to send the command to.</param>
        /// <param name="command">The command to send.</param>
        public static void SendToClient(Player player, CommandBase command)
        {
            SendToClient(player.NetPeer, command);
        }

        /// <summary>
        /// This method is used to send a command to all connected clients.
        /// Does only work if the current game acts as a server.
        /// </summary>
        /// <param name="command">The command to send.</param>
        public static void SendToClients(CommandBase command)
        {
            if (MultiplayerManager.Instance.CurrentRole != MultiplayerRole.Server)
                return;

            byte id = _cmdMapping[command.GetType()];
            MultiplayerManager.Instance.CurrentServer.SendToClients(id, command);
        }

        /// <summary>
        /// This method is used to send a command to all connected clients except the excluded player.
        /// Does only work if the current game acts as a server.
        /// </summary>
        /// <param name="command">The command to send.</param>
        /// <param name="exclude">The player to not send the packet to.</param>
        public static void SendToOtherClients(CommandBase command, Player exclude)
        {
            foreach (Player player in MultiplayerManager.Instance.CurrentServer.ConnectedPlayers.Values)
            {
                if (player.Equals(exclude))
                    continue;

                SendToClient(player, command);
            }
        }

        /// <summary>
        /// This method is used to send a command to the server.
        /// Does only work if the current game acts as a client.
        /// </summary>
        /// <param name="command">The command to send.</param>
        public static void SendToServer(CommandBase command)
        {
            if (MultiplayerManager.Instance.CurrentClient.Status == ClientStatus.Disconnected)
                return;

            byte id = _cmdMapping[command.GetType()];
            MultiplayerManager.Instance.CurrentClient.SendToServer(id, command);
        }

        /// <summary>
        /// This method is used to send a command to a connected partner.
        /// If the current game acts as a server, the command is sent to all clients.
        /// If it acts as a client, the command is sent to the server.
        /// </summary>
        /// <param name="command">The command to send.</param>
        public static void SendToAll(CommandBase command)
        {
            // Check if this command belongs to a transaction
            TransactionHandler.CheckSendTransaction(command);

            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
            {
                SendToServer(command);
            }
            else if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
            {
                SendToClients(command);
            }
        }

        /// <summary>
        /// This method is used to handle a connecting client.
        /// It calls the OnClientConnect methods of all handlers.
        /// </summary>
        /// <param name="player">The connected player.</param>
        public static void HandleClientConnect(Player player)
        {
            foreach (CommandHandler handler in _handlerMapping.Values)
            {
                handler.OnClientConnect(player);
            }
        }

        /// <summary>
        /// This method is used to handle a disconnecting client.
        /// It calls the OnClientDisconnect methods of all handlers.
        /// </summary>
        /// <param name="player">The disconnected player.</param>
        public static void HandleClientDisconnect(Player player)
        {
            foreach (CommandHandler handler in _handlerMapping.Values)
            {
                handler.OnClientDisconnect(player);
            }
        }

        /// <summary>
        /// This method is used to get the id of a given command.
        /// </summary>
        /// <param name="commandType">The Type of a CommandBase subclass.</param>
        /// <returns>The id of the given command.</returns>
        public static byte GetCommandId(Type commandType)
        {
            return _cmdMapping[commandType];
        }

        /// <summary>
        /// This method is used to get the handler of given command.
        /// </summary>
        /// <param name="commandType">The Type of a CommandBase subclass.</param>
        /// <returns>The handler for the given command.</returns>
        public static CommandHandler GetCommandHandler(Type commandType)
        {
            _handlerMapping.TryGetValue(GetCommandId(commandType), out CommandHandler handler);
            return handler;
        }

        static Command()
        {
            // Get all CommandHandler subclasses in the CSM.Commands.Handler namespace
            Type[] handlers = typeof(Command).Assembly.GetTypes()
              .Where(t => String.Equals(t.Namespace, "CSM.Commands.Handler", StringComparison.Ordinal))
              .Where(t => t.IsSubclassOf(typeof(CommandHandler)))
              .Where(t => !t.IsAbstract)
              .ToArray();

            // Create instances of the handlers and initialize mappings
            foreach (Type type in handlers)
            {
                CommandHandler handler = (CommandHandler)Activator.CreateInstance(type);
                _handlerMapping.Add(handler.ID, handler);
                _cmdMapping.Add(handler.GetDataType(), handler.ID);
            }
        }
    }
}