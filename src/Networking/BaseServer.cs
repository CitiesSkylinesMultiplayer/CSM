using CSM.Commands;
using CSM.Helpers;
using CSM.Networking.Config;
using CSM.Networking.Status;
using CSM.Panels;
using CSM.Util;
using LiteNetLib;
using Open.Nat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CSM.Networking
{
    /// <summary>
    ///     Server
    /// </summary>
    public class BaseServer
    {
        // The server
        protected LiteNetLib.NetManager _netServer;

        // Connected clients
        public Dictionary<int, Player> ConnectedPlayers { get; } = new Dictionary<int, Player>();

        // Config options for server
        public ServerConfig Config { get; protected set; }

        /// <summary>
        ///     The current status of the server
        /// </summary>
        public ServerStatus Status { get; protected set; }

        public BaseServer()
        {
            // Set up network items
            EventBasedNetListener listener = new EventBasedNetListener();
            _netServer = new LiteNetLib.NetManager(listener);

            // Listen to events
            listener.NetworkReceiveEvent += ListenerOnNetworkReceiveEvent;
            listener.NetworkErrorEvent += ListenerOnNetworkErrorEvent;
            listener.PeerDisconnectedEvent += ListenerOnPeerDisconnectedEvent;
            listener.NetworkLatencyUpdateEvent += ListenerOnNetworkLatencyUpdateEvent;
            listener.ConnectionRequestEvent += ListenerOnConnectionRequestEvent;
        }

        /// <summary>
        ///     Starts the server with the specified config options
        /// </summary>
        /// <param name="serverConfig">Server config information</param>
        /// <returns>If the server has started.</returns>
        public virtual bool StartServer(ServerConfig serverConfig)
        {
            // If the server is already running, we will stop and start it again
            if (Status == ServerStatus.Running)
                StopServer();

            // Set the config
            Config = serverConfig;

            // Let the user know that we are trying to start the server
            Log.Info($"Attempting to start server on port {Config.Port}...");

            // Attempt to start the server
            bool result = _netServer.Start(Config.Port);

            // If the server has not started, tell the user and return false.
            if (!result)
            {
                Log.Error("The server failed to start.");
                StopServer(); // Make sure the server is fully stopped
                return false;
            }

            try
            {
                // This async stuff is nasty, but we have to target .net 3.5 (unless cities skylines upgrades to something higher).
                NatDiscoverer nat = new NatDiscoverer();
                CancellationTokenSource cts = new CancellationTokenSource();
                cts.CancelAfter(5000);

                nat.DiscoverDeviceAsync(PortMapper.Upnp, cts).ContinueWith(task => task.Result.CreatePortMapAsync(new Mapping(Protocol.Udp, Config.Port,
                    Config.Port, "Cities Skylines Multiplayer (UDP)"))).Wait();
            }
            catch (Exception e)
            {
                Log.Error($"Failed to automatically open port. Manual Port Forwarding is required: {e.Message}");
                ChatLogPanel.PrintGameMessage(ChatLogPanel.MessageType.Error, "Failed to automatically open port. Manual port forwarding is required.");
            }

            // Update the status
            Status = ServerStatus.Running;

            // Update the console to let the user know the server is running
            Log.Info("The server has started.");
            ChatLogPanel.PrintGameMessage("The server has started.");
            return true;
        }

        /// <summary>
        ///     Stops the server
        /// </summary>
        public void StopServer()
        {
            // Update status and stop the server
            Status = ServerStatus.Stopped;
            _netServer.Stop();

            MultiplayerManager.Instance.PlayerList.Clear();
            TransactionHandler.ClearTransactions();
            ToolSimulator.Clear();

            Log.Info("Server stopped.");
        }

        /// <summary>
        ///     Send a message to all connected clients.
        /// </summary>
        /// <param name="message">The actual message</param>
        public void SendToClients(CommandBase message)
        {
            if (Status != ServerStatus.Running)
                return;

            _netServer.SendToAll(message.Serialize(), DeliveryMethod.ReliableOrdered);

            Log.Debug($"Sending {message.GetType().Name} to all clients");
        }

        /// <summary>
        ///     Send a message to a specific client
        /// </summary>
        public void SendToClient(NetPeer peer, CommandBase message)
        {
            if (Status != ServerStatus.Running)
                return;

            peer.Send(message.Serialize(), DeliveryMethod.ReliableOrdered);

            Log.Debug($"Sending {message.GetType().Name} to client at {peer.EndPoint.Address}:{peer.EndPoint.Port}");
        }

        /// <summary>
        ///     Polls new events from the clients.
        /// </summary>
        public void ProcessEvents()
        {
            // Poll for new events
            _netServer.PollEvents();
        }

        /// <summary>
        ///     When we get a message from a client, we handle the message here
        ///     and perform any necessary tasks.
        /// </summary>
        private void ListenerOnNetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            try
            {
                // Parse this message
                bool relayOnServer = CommandReceiver.Parse(reader, peer);

                if (relayOnServer)
                {
                    // Copy relevant message part (exclude protocol headers)
                    byte[] data = new byte[reader.UserDataSize];
                    Array.Copy(reader.RawData, reader.UserDataOffset, data, 0, reader.UserDataSize);

                    // Send this message to all other clients
                    List<NetPeer> peers = _netServer.ConnectedPeerList;
                    foreach (NetPeer client in peers)
                    {
                        // Don't send the message back to the client that sent it.
                        if (client.Id == peer.Id)
                            continue;

                        // Send the message so the other client can stay in sync
                        client.Send(data, DeliveryMethod.ReliableOrdered);
                    }
                }
            }
            catch (Exception ex)
            {
                ChatLogPanel.PrintGameMessage(ChatLogPanel.MessageType.Error, "Error while parsing command. See log.");
                Log.Error($"Encountered an error while reading command from {peer.EndPoint.Address}:{peer.EndPoint.Port}:", ex);
            }
        }

        private void ListenerOnNetworkLatencyUpdateEvent(NetPeer peer, int latency)
        {
            if (!ConnectedPlayers.TryGetValue(peer.Id, out Player player))
                return;

            player.Latency = latency;
        }

        private void ListenerOnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (!ConnectedPlayers.TryGetValue(peer.Id, out Player player))
                return;

            Log.Info($"Player {player.Username} lost connection! Reason: {disconnectInfo.Reason}");

            switch (disconnectInfo.Reason)
            {
                case DisconnectReason.RemoteConnectionClose:
                    ChatLogPanel.PrintGameMessage($"Player {player.Username} disconnected!");
                    break;

                case DisconnectReason.Timeout:
                    ChatLogPanel.PrintGameMessage($"Player {player.Username} timed out!");
                    break;

                default:
                    ChatLogPanel.PrintGameMessage($"Player {player.Username} lost connection!");
                    break;
            }

            HandlePlayerDisconnect(player);
        }

        private void ListenerOnConnectionRequestEvent(ConnectionRequest request)
        {
            request.AcceptIfKey("CSM");
        }

        public void HandlePlayerConnect(Player player)
        {
            Log.Info($"Player {player.Username} has connected!");
            ChatLogPanel.PrintGameMessage($"Player {player.Username} has connected!");
            MultiplayerManager.Instance.PlayerList.Add(player.Username);
            Command.HandleClientConnect(player);
        }

        public void HandlePlayerDisconnect(Player player)
        {
            MultiplayerManager.Instance.PlayerList.Remove(player.Username);
            this.ConnectedPlayers.Remove(player.NetPeer.Id);
            Command.HandleClientDisconnect(player);
            TransactionHandler.ClearTransactions(player.NetPeer.Id);
            ToolSimulator.RemoveSender(player.NetPeer.Id);
        }

        /// <summary>
        ///     Called whenever an error happens, we
        ///     write it to the log file.
        /// </summary>
        private void ListenerOnNetworkErrorEvent(IPEndPoint endpoint, SocketError socketError)
        {
            Log.Error($"Received an error from {endpoint.Address}:{endpoint.Port}. Code: {socketError}");
        }

        /// <summary>
        ///     Get the Player object by username. Warning, expensive call!!!
        /// </summary>
        public virtual Player GetPlayerByUsername(string username)
        {
            return ConnectedPlayers.Single(z => z.Value.Username == username).Value;
        }
    }
}
