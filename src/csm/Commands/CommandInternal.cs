using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Networking;
using CSM.API.Networking.Status;
using CSM.Helpers;
using CSM.Models;
using CSM.Mods;
using CSM.Networking;
using LiteNetLib;
using ProtoBuf.Meta;
using UnityEngine;

namespace CSM.Commands
{
    public class CommandInternal
    {
        public static CommandInternal Instance;

        private readonly Dictionary<Type, CommandHandler> _cmdMapping = new Dictionary<Type, CommandHandler>();

        public TypeModel Model { get; private set; }

        /// <summary>
        ///     This method is used to send a command to a connected client.
        ///     Does only work if the current game acts as a server.
        /// </summary>
        /// <param name="peer">The NetPeer to send the command to.</param>
        /// <param name="command">The command to send.</param>
        public void SendToClient(NetPeer peer, CommandBase command)
        {
            if (MultiplayerManager.Instance.CurrentRole != MultiplayerRole.Server)
                return;

            TransactionHandler.StartTransaction(command);
            SetSenderId(command);

            MultiplayerManager.Instance.CurrentServer.SendToClient(peer, command);
        }

        /// <summary>
        ///     This method is used to send a command to a connected client.
        ///     Does only work if the current game acts as a server.
        /// </summary>
        /// <param name="player">The Player to send the command to.</param>
        /// <param name="command">The command to send.</param>
        public void SendToClient(Player player, CommandBase command)
        {
            SendToClient(player.NetPeer, command);
        }

        /// <summary>
        ///     This method is used to send a command to all connected clients.
        ///     Does only work if the current game acts as a server.
        /// </summary>
        /// <param name="command">The command to send.</param>
        public void SendToClients(CommandBase command)
        {
            if (MultiplayerManager.Instance.CurrentRole != MultiplayerRole.Server)
                return;

            TransactionHandler.StartTransaction(command);
            SetSenderId(command);

            MultiplayerManager.Instance.CurrentServer.SendToClients(command);
        }

        /// <summary>
        ///     This method is used to send a command to all connected clients except the excluded player.
        ///     Does only work if the current game acts as a server.
        /// </summary>
        /// <param name="command">The command to send.</param>
        /// <param name="exclude">The player to not send the packet to.</param>
        public void SendToOtherClients(CommandBase command, Player exclude)
        {
            foreach (Player player in MultiplayerManager.Instance.CurrentServer.ConnectedPlayers.Values)
            {
                if (player.Equals(exclude))
                    continue;

                SendToClient(player, command);
            }
        }

        /// <summary>
        ///     This method is used to send a command to the server.
        ///     Does only work if the current game acts as a client.
        /// </summary>
        /// <param name="command">The command to send.</param>
        public void SendToServer(CommandBase command)
        {
            if (MultiplayerManager.Instance.CurrentClient.Status == ClientStatus.Disconnected ||
                MultiplayerManager.Instance.CurrentClient.Status == ClientStatus.PreConnecting)
                return;

            TransactionHandler.StartTransaction(command);
            SetSenderId(command);

            MultiplayerManager.Instance.CurrentClient.SendToServer(command);
        }

        /// <summary>
        ///     This method is used to send a command to a connected partner.
        ///     If the current game acts as a server, the command is sent to all clients.
        ///     If it acts as a client, the command is sent to the server.
        /// </summary>
        /// <param name="command">The command to send.</param>
        public void SendToAll(CommandBase command)
        {
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
        ///     Sets the client/server id of the command.
        /// </summary>
        /// <param name="command">The command to modify.</param>
        private void SetSenderId(CommandBase command)
        {
            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
            {
                command.SenderId = -1;
            }
            else
            {
                command.SenderId = MultiplayerManager.Instance.CurrentClient.ClientId;
            }
        }

        /// <summary>
        ///     This method is used to handle a connecting client.
        ///     It calls the OnClientConnect methods of all handlers.
        /// </summary>
        /// <param name="player">The connected player.</param>
        public void HandleClientConnect(Player player)
        {
            foreach (CommandHandler handler in _cmdMapping.Values)
            {
                handler.OnClientConnect(player);
            }
        }

        /// <summary>
        ///     This method is used to handle a disconnecting client.
        ///     It calls the OnClientDisconnect methods of all handlers.
        /// </summary>
        /// <param name="player">The disconnected player.</param>
        public void HandleClientDisconnect(Player player)
        {
            foreach (CommandHandler handler in _cmdMapping.Values)
            {
                handler.OnClientDisconnect(player);
            }
        }

        /// <summary>
        ///     This method is used to get the handler of given command.
        /// </summary>
        /// <param name="commandType">The Type of a CommandBase subclass.</param>
        /// <returns>The handler for the given command.</returns>
        public CommandHandler GetCommandHandler(Type commandType)
        {
            _cmdMapping.TryGetValue(commandType, out CommandHandler handler);
            return handler;
        }

        public void RefreshModel()
        {
            _cmdMapping.Clear();
            try
            {
                // First, find all CSM classes, then the other mods. This is necessary to 
                // ensure that our management packets have always the same ids,
                // as for example the ConnectionRequest and ConnectionResult commands are used
                // to determine the installed mods.
                IEnumerable<Type> packets = AssemblyHelper.FindClassesInCSM(typeof(CommandHandler));
                foreach (Connection connection in ModSupport.Instance.ConnectedMods.OrderBy(mod => mod.Name))
                {
                    foreach (Assembly assembly in connection.CommandAssemblies.OrderBy(assembly => assembly.FullName))
                    {
                        packets = packets.Concat(AssemblyHelper.FindImplementationsInAssembly(assembly, typeof(CommandHandler)));
                    }
                }

                Type[] handlers = packets.ToArray();
                Log.Info($"Initializing data model with {handlers.Length} commands...");

                // Create a protobuf model
                RuntimeTypeModel model = RuntimeTypeModel.Create();

                // Set type surrogates
                model[typeof(Vector3)].SetSurrogate(typeof(Vector3Surrogate));
                model[typeof(NetTool.ControlPoint)].SetSurrogate(typeof(ControlPointSurrogate));

                // Add Quaternion Surrogate
                model[typeof(Quaternion)].SetSurrogate(typeof(QuaternionSurrogate));

                // Add Color Surrogate
                model[typeof(Color)].SetSurrogate(typeof(ColorSurrogate));

                // Add base command to the protobuf model with all attributes
                model.Add(typeof(CommandBase), true);
                MetaType baseCmd = model[typeof(CommandBase)];

                // Lowest id of the subclasses
                int id = 100;

                // Create instances of the handlers, initialize mappings and register command subclasses in the protobuf model
                foreach (Type type in handlers)
                {
                    CommandHandler handler = (CommandHandler)Activator.CreateInstance(type);
                    _cmdMapping.Add(handler.GetDataType(), handler);

                    // Add subtype to the protobuf model with all attributes
                    baseCmd.AddSubType(id, handler.GetDataType());
                    model.Add(handler.GetDataType(), true);

                    id++;
                }

                // Compile the protobuf model
                model.CompileInPlace();

                Model = model;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to initialize data model", ex);
            }
        }

        public CommandInternal()
        {
            Command.ConnectToCSM(this.SendToAll, this.SendToServer, this.SendToClients, this.GetCommandHandler);
        }
    }
}
