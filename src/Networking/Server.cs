using ColossalFramework;
using CSM.Commands;
using CSM.Extensions;
using CSM.Helpers;
using CSM.Networking.Config;
using CSM.Networking.Status;
using CSM.Events;
using LiteNetLib;
using LiteNetLib.Utils;
using Open.Nat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace CSM.Networking
{
    /// <summary>
    ///     Server
    /// </summary>
    public class Server
    {
        // The client timeout in seconds
        private const int TIMEOUT = 15;

        // The server
        public LiteNetLib.NetManager NetServer { get; }

        // Run a background processing thread
        private Thread _serverProcessingThread;

        // Timer for handling ping
        private System.Timers.Timer _pingTimer;

        // Connected clients
        private readonly Dictionary<long, Player> _connectedClients = new Dictionary<long, Player>();

        // The player instance for the host player (TODO: Make name configurable)
        private Player _hostPlayer = new Player("Host player");

        // Config options for server
        public ServerConfig Config { get; private set; }

        /// <summary>
        ///     The current status of the server
        /// </summary>
        public ServerStatus Status { get; private set; }

        public event PlayerConnectEventHandler ClientConnect;
        public event PlayerDisconnectEventHandler ClientDisconnect;

        public Server()
        {
            // Set up network items
            var listener = new EventBasedNetListener();
            NetServer = new LiteNetLib.NetManager(listener, "Tango");

            // Listen to events
            listener.NetworkReceiveEvent += ListenerOnNetworkReceiveEvent;
            listener.NetworkErrorEvent += ListenerOnNetworkErrorEvent;

            // Setup timer
            _pingTimer = new System.Timers.Timer();
            _pingTimer.Elapsed += OnPing;
            _pingTimer.Interval = 100;
            _pingTimer.Start();

            ClientConnect += (Server server, PlayerEventArgs args) => {
                CSM.Log($"Player {args.Player.Username} has connected!");
                SendToClients(CommandBase.ClientConnectCommandId, new ClientConnectCommand { Username = args.Player.Username });
                MultiplayerManager.Instance.PlayerList.Add(args.Player.Username);
            };

            ClientDisconnect += (Server server, PlayerEventArgs args) => {
                CSM.Log($"Player {args.Player.Username} has disconnected!");
                SendToClients(CommandBase.ClientDisconnectCommandId, new ClientDisconnectCommand { Username = args.Player.Username });
                MultiplayerManager.Instance.PlayerList.Remove(args.Player.Username);
            };
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
            NetServer.DiscoveryEnabled = true;
            var result = NetServer.Start(Config.Port);

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

            // Set up processing thread
            _serverProcessingThread = new Thread(ProcessEvents);
            _serverProcessingThread.Start();

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
            NetServer.Stop();

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

            NetServer.SendToAll(ArrayHelpers.PrependByte(messageId, message.Serialize()), SendOptions.ReliableOrdered);
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
        ///     Runs in the background of the game (another thread), polls for new updates
        ///     from the clients.
        /// </summary>
        private void ProcessEvents()
        {
            while (Status == ServerStatus.Running)
            {
                // Poll for new events
                NetServer.PollEvents();

                // Wait
                Thread.Sleep(15);
            }
        }

        /// <summary>
        ///     Ping all connected clients
        /// </summary>
        private void OnPing(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Server not running, don't send ping
            if (Status == ServerStatus.Stopped)
                return;

            // Timeout clients if they are not responding
            DateTime now = DateTime.UtcNow;
            foreach (KeyValuePair<long, Player> player in _connectedClients) {
                if (player.Value.LastPing.AddSeconds(TIMEOUT) < now) {
                    CSM.Log($"Player {player.Value.Username} has timed out!");

                    this._connectedClients.Remove(player.Key);
                    NetServer.DisconnectPeer(player.Value.NetPeer);
                    this.OnClientDisconnected(player.Value);
                }
            }

            // Loop though all connected peers
            foreach (var player in _connectedClients.Values)
            {
                // Send a ping
                player.NetPeer.Send(ArrayHelpers.PrependByte(CommandBase.PingCommandId, new PingCommand().Serialize()), SendOptions.ReliableOrdered);
            }
        }

        /// <summary>
        ///     When we get a message from a client, we handle the message here
        ///     and perform any necessary tasks.
        /// </summary>
        private void ListenerOnNetworkReceiveEvent(NetPeer peer, NetDataReader reader)
        {
            try
            {
                // The message type is the first byte, (255 message types)
                var messageType = reader.Data[0];

                // Skip the first byte
                var message = reader.Data.Skip(1).ToArray();

                // Make sure we know about the connected client
                if (messageType != CommandBase.ConnectionRequestCommandId && !this._connectedClients.ContainsKey(peer.ConnectId)) {
                    CSM.Log($"Client from {peer.EndPoint.Host}:{peer.EndPoint.Port} tried to send packet but never joined with a ConnectionRequestCommand packet. Ignoring...");
                    return;
                }

                Player player = null;
                if (messageType != CommandBase.ConnectionRequestCommandId) {
                    player = this._connectedClients[peer.ConnectId];
                }

                // Switch between all the messages
                switch (messageType)
                {
                    case CommandBase.ConnectionRequestCommandId:

                        var connectionResult = CommandBase.Deserialize<ConnectionRequestCommand>(message);

                        CSM.Log($"Connection request from {peer.EndPoint.Host}:{peer.EndPoint.Port}. Version: {connectionResult.GameVersion}, ModCount: {connectionResult.ModCount}, ModVersion: {connectionResult.ModVersion}");

                        // TODO check these values, but for now, just accept the request.
                        // TODO check if the username already exists

                        var newPlayer = new Player(peer, connectionResult.Username);
                        this._connectedClients[peer.ConnectId] = newPlayer;

                        SendToClient(peer, CommandBase.ConnectionResultCommandId, new ConnectionResultCommand { Success = true });

                        // Send current player list (without the newly joined player)
                        SendToClient(peer, CommandBase.PlayerListCommand, new PlayerListCommand { PlayerList = MultiplayerManager.Instance.PlayerList });

                        this.OnClientConnected(newPlayer);
                        break;

                    case CommandBase.ConnectionCloseCommandId:

                        this._connectedClients.Remove(peer.ConnectId);

                        this.OnClientDisconnected(player);

                        // Send quit confirmation
                        SendToClient(peer, CommandBase.ConnectionCloseCommandId, new ConnectionCloseCommand());

                        break;
                    case CommandBase.PingCommandId:
                        player.LastPing = DateTime.UtcNow;
                        break;
                    case CommandBase.PauseCommandID:
                        var pause = CommandBase.Deserialize<PauseCommand>(message);
                        SimulationManager.instance.SimulationPaused = pause.SimulationPaused;
                        break;

                    case CommandBase.SpeedCommandID:
                        var speed = CommandBase.Deserialize<SpeedCommand>(message);
                        SimulationManager.instance.SelectedSimulationSpeed = speed.SelectedSimulationSpeed;
                        break;

                    case CommandBase.MoneyCommandID:
                        var internalMoney = CommandBase.Deserialize<MoneyCommand>(message);
                        typeof(EconomyManager).GetField("m_cashAmount", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, internalMoney.InternalMoneyAmount);
                        typeof(EconomyManager).GetField("m_lastCashAmount", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, internalMoney.InternalMoneyAmount);
                        break;

                    case CommandBase.BuildingCreatedCommandID:
                        var Buildings = CommandBase.Deserialize<BuildingCreatedCommand>(message);
                        BuildingInfo info = PrefabCollection<BuildingInfo>.GetPrefab(Buildings.Infoindex);
                        BuildingExtension.LastPosition = Buildings.Position;
                        Singleton<BuildingManager>.instance.CreateBuilding(out ushort building, ref Singleton<SimulationManager>.instance.m_randomizer, info, Buildings.Position, Buildings.Angle, Buildings.Length, Singleton<SimulationManager>.instance.m_currentBuildIndex);
                        break;

                    case CommandBase.BuildingRemovedCommandID:
                        var BuildingRemovedPosition = CommandBase.Deserialize<BuildingRemovedCommand>(message);
                        long num = Mathf.Clamp((int)((BuildingRemovedPosition.Position.x / 64f) + 135f), 0, 0x10d);  //The buildingID is stored in the M_buildingGrid[] which is calculated by thís arbitrary calculation using the buildings position
                        long index = (Mathf.Clamp((int)((BuildingRemovedPosition.Position.z / 64f) + 135f), 0, 0x10d) * 270) + num;
                        var BuildingId = BuildingManager.instance.m_buildingGrid[index];
                        if (BuildingId != 0)
                        {
                            BuildingManager.instance.ReleaseBuilding(BuildingId);
                        }
                        break;

					case CommandBase.BuildingRelocatedCommandID:
						var BuildingRelocationData = CommandBase.Deserialize<BuildingRelocationCommand>(message);
						long num2 = Mathf.Clamp((int)((BuildingRelocationData.OldPosition.x / 64f) + 135f), 0, 0x10d); //The buildingID is stored in the M_buildingGrid[index] which is calculated by thís arbitrary calculation using the buildings position
						long index2 = (Mathf.Clamp((int)((BuildingRelocationData.OldPosition.z / 64f) + 135f), 0, 0x10d) * 270) + num2;
						var BuildingId2 = BuildingManager.instance.m_buildingGrid[index2];
						Singleton<BuildingManager>.instance.RelocateBuilding(BuildingId2, BuildingRelocationData.NewPosition, BuildingRelocationData.Angle);
						break;
				}
            }
            catch (Exception ex)
            {
                CSM.Log($"Received an error from {peer.EndPoint.Host}:{peer.EndPoint.Port}. Message: {ex.Message}");
            }
        }

        private void OnClientConnected(Player player)
        {
            if (ClientConnect != null) {
                ClientConnect(this, new PlayerEventArgs(player));
            }
        }

        private void OnClientDisconnected(Player player)
        {
            if (ClientDisconnect != null) {
                ClientDisconnect(this, new PlayerEventArgs(player));
            }
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