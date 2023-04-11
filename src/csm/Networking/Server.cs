using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Networking;
using CSM.API.Networking.Status;
using CSM.BaseGame.Helpers;
using CSM.Commands;
using CSM.GS.Commands;
using CSM.GS.Commands.Data.ApiServer;
using CSM.Helpers;
using CSM.Networking.Config;
using CSM.Util;
using LiteNetLib;
using ColossalFramework;
using CSM.BaseGame.Injections.Tools;
using Open.Nat;
using CommandReceiver = CSM.Commands.CommandReceiver;

namespace CSM.Networking
{
    /// <summary>
    ///     Server
    /// </summary>
    public class Server
    {
        // The server
        private readonly LiteNetLib.NetManager _netServer;

        // Keep alive tick tracker
        private int _keepAlive = 1;
        
        // Connected clients
        public Dictionary<int, Player> ConnectedPlayers { get; } = new Dictionary<int, Player>();

        /// <summary>
        ///     Get the Player object of the server host
        /// </summary>
        public Player HostPlayer { get { return _hostPlayer; } }
        // The player instance for the host player
        private Player _hostPlayer;

        // Config options for server
        public ServerConfig Config { get; private set; }

        /// <summary>
        ///     The current status of the server
        /// </summary>
        public ServerStatus Status { get; private set; }

        /// <summary>
        ///     The server token of the current server.
        /// </summary>
        public string ServerToken { get; private set; }

        /// <summary>
        ///     If the port was forwarded automatically.
        /// </summary>
        public bool AutomaticSuccess { get; private set; }

        public Server()
        {
            // Set up network items
            EventBasedNetListener listener = new EventBasedNetListener();
            _netServer = new LiteNetLib.NetManager(listener)
            {
                NatPunchEnabled = true,
                UnconnectedMessagesEnabled = true
            };

            // Listen to events
            listener.NetworkReceiveEvent += ListenerOnNetworkReceiveEvent;
            listener.NetworkReceiveUnconnectedEvent += ListenerOnNetworkReceiveUnconnectedEvent;
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
        public bool StartServer(ServerConfig serverConfig)
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

            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            ServerToken = new string(Enumerable.Repeat(chars, 20)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            // First strategy for NAT traversal: Hole punching
            SetupHolePunching();

            // Second strategy for NAT traversal: Upnp
            try
            {
                NatDiscoverer nat = new NatDiscoverer();
                nat.DiscoverDeviceAsync().ContinueWith(task => task.Result.CreatePortMapAsync(new Mapping(Protocol.Udp, Config.Port,
                    Config.Port, "Cities Skylines Multiplayer (UDP)"))).Wait();
                AutomaticSuccess = true;
            }
            catch (Exception)
            {
                AutomaticSuccess = false;
            }

            // Check port forwarding status
            SendToApiServer(new PortCheckRequestCommand { Port = Config.Port });

            // Update the status
            Status = ServerStatus.Running;

            // Initialize host player
            _hostPlayer = new Player(Config.Username);
            _hostPlayer.Status = ClientStatus.Connected;
            MultiplayerManager.Instance.PlayerList.Add(_hostPlayer.Username);

            // Set the steam presence 'connect' key. This allows users to click "Join Game" within the steam overlay.
            if (CSM.IsSteamPresent)
            {
                SteamHelpers.Instance.SetPlayingOnServer(ServerToken);
                SteamHelpers.Instance.SetGroupSize(1);
            }

            // Update the console to let the user know the server is running
            Log.Info("The server has started.");
            Chat.Instance.PrintGameMessage("The server has started. Checking if it is reachable from the internet...");
            return true;
        }

        private void SetupHolePunching()
        {
            EventBasedNatPunchListener natPunchListener = new EventBasedNatPunchListener();
            natPunchListener.NatIntroductionSuccess += (point, type, token) =>
            {
                Log.Debug("Nat introduction from " + point);
            };

            _netServer.NatPunchModule.Init(natPunchListener);

            // Register on server
            string localIp = NetUtils.GetLocalIp(LocalAddrType.IPv4);
            if (string.IsNullOrEmpty(localIp))
                localIp = NetUtils.GetLocalIp(LocalAddrType.IPv6);

            SendToApiServer(new ServerRegistrationCommand
            {
                LocalIp = localIp,
                LocalPort = Config.Port,
                Token = ServerToken
            });
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
            Singleton<ToolSimulator>.instance.Clear();

            // Clear steam presence. This prevents users from clicking "Join Game".
            if (CSM.IsSteamPresent)
            {
                SteamHelpers.Instance.ClearRichPresence();
            }

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

            _netServer.SendToAll(Serializer.Serialize(message), DeliveryMethod.ReliableOrdered);

            Log.Debug($"Sending {message.GetType().Name} to all clients");
        }

        /// <summary>
        ///     Send a message to a specific client
        /// </summary>
        public void SendToClient(NetPeer peer, CommandBase message)
        {
            if (Status != ServerStatus.Running)
                return;

            peer.Send(Serializer.Serialize(message), DeliveryMethod.ReliableOrdered);

            Log.Debug($"Sending {message.GetType().Name} to client at {peer.EndPoint.Address}:{peer.EndPoint.Port}");
        }

        /// <summary>
        ///     Polls new events from the clients.
        /// </summary>
        public void ProcessEvents()
        {
            // Poll for new events
            _netServer.NatPunchModule.PollEvents();
            _netServer.PollEvents();
            // Send keepalive to GS
            if (_keepAlive % (60 * 5) == 0)
            {
                string localIp = NetUtils.GetLocalIp(LocalAddrType.IPv4);
                if (string.IsNullOrEmpty(localIp))
                    localIp = NetUtils.GetLocalIp(LocalAddrType.IPv6);

                SendToApiServer(new ServerRegistrationCommand
                {
                    LocalIp = localIp,
                    LocalPort = Config.Port,
                    Token = ServerToken
                });
            }
            _keepAlive += 1;
        }

        /// <summary>
        ///     Send a message to the API server
        /// </summary>
        /// <param name="message"></param>
        public void SendToApiServer(ApiCommandBase message)
        {
            try
            {
                IPAddress apiServer = IpAddress.GetIpv4(CSM.Settings.ApiServer);
                _netServer.SendUnconnectedMessage(ApiCommand.Serialize(message),
                    new IPEndPoint(apiServer, CSM.Settings.ApiServerPort));
                Log.Debug(
                    $"Sending {message.GetType().Name} to API server at {apiServer}:{CSM.Settings.ApiServerPort}");
            }
            catch (Exception e)
            {
                Log.Warn($"Could not send message to API server at {CSM.Settings.ApiServer}:{CSM.Settings.ApiServerPort}: {e}");
            }
        }

        /// <summary>
        ///     Receive messages from the API server.
        /// </summary>
        private void ListenerOnNetworkReceiveUnconnectedEvent(IPEndPoint from, NetPacketReader reader, UnconnectedMessageType type)
        {
            if (type != UnconnectedMessageType.BasicMessage)
                return;

            try
            {
                // Only allow responses from the API server
                if (!Equals(from.Address, IpAddress.GetIpv4(CSM.Settings.ApiServer)))
                    return;

                GS.Commands.CommandReceiver.Parse(reader);
            }
            catch (Exception ex)
            {
                Chat.Instance.PrintGameMessage(Chat.MessageType.Error, "Error while parsing unconnected message. See log.");
                Log.Error($"Encountered an error while reading command from {from.Address}:{from.Port}:", ex);
            }
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
                Chat.Instance.PrintGameMessage(Chat.MessageType.Error, "Error while parsing command. See log.");
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
                    Chat.Instance.PrintGameMessage($"Player {player.Username} disconnected!");
                    break;

                case DisconnectReason.Timeout:
                    Chat.Instance.PrintGameMessage($"Player {player.Username} timed out!");
                    break;

                default:
                    Chat.Instance.PrintGameMessage($"Player {player.Username} lost connection!");
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
            Chat.Instance.PrintGameMessage($"Player {player.Username} has connected!");
            MultiplayerManager.Instance.PlayerList.Add(player.Username);
            CommandInternal.Instance.HandleClientConnect(player);
            if (CSM.IsSteamPresent)
            {
                SteamHelpers.Instance.SetGroupSize(MultiplayerManager.Instance.PlayerList.Count);
            }
        }

        public void HandlePlayerDisconnect(Player player)
        {
            MultiplayerManager.Instance.PlayerList.Remove(player.Username);
            this.ConnectedPlayers.Remove(player.NetPeer.Id);
            CommandInternal.Instance.HandleClientDisconnect(player);
            TransactionHandler.ClearTransactions(player.NetPeer.Id);
            Singleton<ToolSimulator>.instance.RemoveSender(player.NetPeer.Id);
            Singleton<ToolSimulatorCursorManager>.instance.RemoveCursorView(player.NetPeer.Id);

            if (CSM.IsSteamPresent)
            {
                SteamHelpers.Instance.SetGroupSize(MultiplayerManager.Instance.PlayerList.Count);
            }
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
        public Player GetPlayerByUsername(string username)
        {
            if (username == HostPlayer.Username)
                return HostPlayer;
            else
                return ConnectedPlayers.Single(z => z.Value.Username == username).Value;
        }
    }
}
