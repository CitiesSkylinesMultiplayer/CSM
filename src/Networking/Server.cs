using CSM.Commands;
using CSM.Commands.Handler;
using CSM.Helpers;
using CSM.Networking.Config;
using CSM.Networking.Status;
using LiteNetLib;
using LiteNetLib.Utils;
using Open.Nat;
using System;
using System.Collections.Generic;
using System.Threading;

namespace CSM.Networking
{
    /// <summary>
    ///     Server
    /// </summary>
    public class Server
    {
        // The server
        private LiteNetLib.NetManager _netServer;

        // Connected clients
        public Dictionary<long, Player> ConnectedPlayers { get; } = new Dictionary<long, Player>();

        // The player instance for the host player
        private Player _hostPlayer;

        // Config options for server
        public ServerConfig Config { get; private set; }

        /// <summary>
        ///     The current status of the server
        /// </summary>
        public ServerStatus Status { get; private set; }

        public Server()
        {
            // Set up network items
            var listener = new EventBasedNetListener();
            _netServer = new LiteNetLib.NetManager(listener, "Tango");

            // Listen to events
            listener.NetworkReceiveEvent += ListenerOnNetworkReceiveEvent;
            listener.NetworkErrorEvent += ListenerOnNetworkErrorEvent;
            listener.PeerDisconnectedEvent += ListenerOnPeerDisconnectedEvent;
            listener.NetworkLatencyUpdateEvent += ListenerOnNetworkLatencyUpdateEvent;
        }

        /// <summary>
        ///     Starts the server with the specified config options
        /// </summary>
        /// <param name="serverConfig">Server config information</param>
        /// <returns>If the server has started.</returns>
        public bool StartServer(ServerConfig serverConfig)
        {
            // If the server is already running, we will stop and start it again
            if (Status == ServerStatus.Running)
                StopServer();

            // Set the config
            Config = serverConfig;

            // Let the user know that we are trying to start the server
            CSM.Log($"Attempting to start server on port {Config.Port}...");

            // Attempt to start the server
            _netServer.DiscoveryEnabled = true;
            var result = _netServer.Start(Config.Port);

            // If the server has not started, tell the user and return false.
            if (!result)
            {
                CSM.Log("The server failed to start.");
                StopServer(); // Make sure the server is fully stopped
                return false;
            }

            try
            {
                // This async stuff is nasty, but we have to target .net 3.5 (unless cities skylines upgrades to something higher).
                var nat = new NatDiscoverer();
                var cts = new CancellationTokenSource();
                cts.CancelAfter(5000);

                nat.DiscoverDeviceAsync(PortMapper.Upnp, cts).ContinueWith(task => task.Result.CreatePortMapAsync(new Mapping(Protocol.Udp, Config.Port,
                    Config.Port, "Cities Skylines Multiplayer (UDP)"))).Wait();
            }
            catch (Exception e)
            {
                CSM.Log("Failed to automatically open port. Manual Port Forwarding is required. " + e.Message);
            }

            // Update the status
            Status = ServerStatus.Running;

            // Initialize host player
            _hostPlayer = new Player(Config.Username);
            MultiplayerManager.Instance.PlayerList.Add(_hostPlayer.Username);

            // Update the console to let the user know the server is running
            CSM.Log("The server has started.");
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

            CSM.Log("Stopped server");
        }

        /// <summary>
        ///     Send a message to all connected clients.
        /// </summary>
        /// <param name="messageId">Message type/id</param>
        /// <param name="message">The actual message</param>
        public void SendToClients(byte messageId, CommandBase message)
        {
            if (Status != ServerStatus.Running)
                return;

            _netServer.SendToAll(ArrayHelpers.PrependByte(messageId, message.Serialize()), SendOptions.ReliableOrdered);
        }

        /// <summary>
        ///     Send a message to a specific client
        /// </summary>
        public void SendToClient(NetPeer peer, byte messageId, CommandBase message)
        {
            if (Status != ServerStatus.Running)
                return;

            peer.Send(ArrayHelpers.PrependByte(messageId, message.Serialize()), SendOptions.ReliableOrdered);
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
        private void ListenerOnNetworkReceiveEvent(NetPeer peer, NetDataReader reader)
        {
            try
            {
                // Handle ConnectionRequest as special case
                if (reader.Data[0] == 0)
                {
                    Command.Parse(reader.Data, out CommandHandler handler, out byte[] message);
                    ConnectionRequestHandler requestHandler = (ConnectionRequestHandler)handler;
                    requestHandler.HandleOnServer(message, peer);
                    return;
                }

                // Parse this message
                ConnectedPlayers.TryGetValue(peer.ConnectId, out Player player);
                Command.ParseOnServer(reader.Data, player);

                // Send this message to all other clients
                var peers = _netServer.GetPeers();
                foreach (var client in peers)
                {
                    // Don't send the message back to the client that sent it.
                    if (client.ConnectId == peer.ConnectId)
                        continue;

                    // Send the message so the other client can stay in sync
                    client.Send(reader.Data, SendOptions.ReliableOrdered);
                }
            }
            catch (Exception ex)
            {
                CSM.Log($"Encountered an error while reading command from {peer.EndPoint.Host}:{peer.EndPoint.Port}:");
                CSM.Log(ex.ToString());
            }
        }

        private void ListenerOnNetworkLatencyUpdateEvent(NetPeer peer, int latency)
        {
            if (!ConnectedPlayers.TryGetValue(peer.ConnectId, out Player player))
                return;

            player.Latency = latency;
        }

        private void ListenerOnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (!ConnectedPlayers.TryGetValue(peer.ConnectId, out Player player))
                return;

            switch (disconnectInfo.Reason)
            {
                case DisconnectReason.RemoteConnectionClose:
                    CSM.Log($"Player {player.Username} disconnected!");
                    break;

                case DisconnectReason.Timeout:
                    CSM.Log($"Player {player.Username} timed out!");
                    break;

                default:
                    CSM.Log($"Player {player.Username} lost connection!");
                    break;
            }

            HandlePlayerDisconnect(player);
        }

        public void HandlePlayerConnect(Player player)
        {
            CSM.Log($"Player {player.Username} has connected!");
            MultiplayerManager.Instance.PlayerList.Add(player.Username);
            Command.HandleClientConnect(player);
        }

        public void HandlePlayerDisconnect(Player player)
        {
            MultiplayerManager.Instance.PlayerList.Remove(player.Username);
            this.ConnectedPlayers.Remove(player.NetPeer.ConnectId);
            Command.HandleClientDisconnect(player);
        }

        /// <summary>
        ///     Called whenever an error happens, we
        ///     log this to the console for now.
        /// </summary>
        private void ListenerOnNetworkErrorEvent(NetEndPoint endpoint, int socketerrorcode)
        {
            CSM.Log($"Received an error from {endpoint.Host}:{endpoint.Port}. Code: {socketerrorcode}");
        }
    }
}