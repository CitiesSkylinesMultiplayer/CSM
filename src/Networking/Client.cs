using ColossalFramework;
using ColossalFramework.Plugins;
using CSM.Commands;
using CSM.Extensions;
using CSM.Helpers;
using CSM.Networking.Config;
using CSM.Networking.Status;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace CSM.Networking
{
    /// <summary>
    ///     Client
    /// </summary>
    public class Client
    {
        // The timeout in seconds
        private const int TIMEOUT = 15;

        // The client
        public LiteNetLib.NetManager NetClient { get; }

        // Run a background processing thread
        private Thread _clientProcessingThread;

        // Configuration options for server
        private ClientConfig _clientConfig;

        // The last time we received a message from the server
        private DateTime _lastServerPing;

        // Timer to make sure the client is already connected to the server
        private System.Timers.Timer _pingTimer;

        /// <summary>
        ///     The current status of the client
        /// </summary>
        public ClientStatus Status { get; private set; }

        /// <summary>
        ///     If the status is disconnected, this will contain
        ///     the reason why.
        /// </summary>
        public string ConnectionMessage { get; private set; }

        public Client()
        {
            // Set up network items
            var listener = new EventBasedNetListener();
            NetClient = new LiteNetLib.NetManager(listener, "Tango");

            // Listen to events
            listener.NetworkReceiveEvent += ListenerOnNetworkReceiveEvent;
            listener.NetworkErrorEvent += ListenerOnNetworkErrorEvent;
            listener.PeerConnectedEvent += ListenerOnPeerConnectedEvent;

            // Setup timer
            _pingTimer = new System.Timers.Timer();
            _pingTimer.Elapsed += OnPing;
            _pingTimer.Interval = 1000;
        }

        /// <summary>
        ///     Attempt to connect to a server
        /// </summary>
        /// <param name="clientConfig">Client config params</param>
        /// <returns>True if the client is connected to the server, false if not</returns>
        public bool Connect(ClientConfig clientConfig)
        {
            // if we are currently trying to connect, cancel
            // and try again.
            if (Status == ClientStatus.Connecting)
                Disconnect();

            // The client is already connected so we need to
            // disconnect.
            if (Status == ClientStatus.Connected)
                Disconnect();

            // Set the config
            _clientConfig = clientConfig;

            // Let the user know that we are trying to connect to a server
            CSM.Log($"Attempting to connect to server at {_clientConfig.HostAddress}:{_clientConfig.Port}...");

            // Start the client, if client setup fails, return out and
            // tell the user
            var result = NetClient.Start();
            if (!result)
            {
                CSM.Log("The client failed to start.");
                ConnectionMessage = "The client failed to start.";
                Disconnect(); // make sure we are fully disconnected
                return false;
            }

            // Try connect to server, update the status to say that
            // we are trying to connect.
            NetClient.Connect(_clientConfig.HostAddress, _clientConfig.Port);

            // Start processing networking
            Status = ClientStatus.Connecting;

            // Setup processing thread
            _clientProcessingThread = new Thread(ProcessEvents);
            _clientProcessingThread.Start();
            _pingTimer.Start();

            // We need to wait in a loop for 30 seconds (waiting 500ms each time)
            // while we wait for a successful connection (Status = Connected) or a
            // failed connection (Status = Disconnected).
            var waitWatch = new Stopwatch();
            waitWatch.Start();

            while (waitWatch.Elapsed < TimeSpan.FromSeconds(30))
            {
                // If we connect, exit the loop and return true
                if (Status == ClientStatus.Connected)
                    return true;

                // The client cannot connect for some reason, the ConnectionMessage
                // variable will contain why.
                if (Status == ClientStatus.Disconnected)
                {
                    Disconnect(); // make sure we are fully disconnected
                    return false;
                }

                // Wait 500ms
                Thread.Sleep(500);
            }

            // We have timed out
            ConnectionMessage = "Could not connect to server, timed out.";
            CSM.Log("Could not connect to server, timed out.");

            // Did not connect
            Disconnect(); // make sure we are fully disconnected
            return false;
        }

        public void RequestDisconnect()
        {
            // Send quit packet if we had a connection
            if (Status == ClientStatus.Connected)
            {
                SendToServer(CommandBase.ConnectionCloseCommandId, new ConnectionCloseCommand());
            }
        }

        /// <summary>
        ///     Attempt to disconnect from the server
        /// </summary>
        /// <returns></returns>
        public void Disconnect()
        {
            // Update status and stop client
            Status = ClientStatus.Disconnected;
            NetClient.Stop();

            _pingTimer.Stop();

            CSM.Log("Disconnected from server.");
        }

        public void SendToServer(byte messageId, CommandBase message)
        {
            if (Status == ClientStatus.Disconnected)
                return;

            NetClient.GetFirstPeer().Send(ArrayHelpers.PrependByte(messageId, message.Serialize()), SendOptions.ReliableOrdered);
        }

        /// <summary>
        ///     Runs in the background of the game (another thread), polls for new updates
        ///     from the server.
        /// </summary>
        private void ProcessEvents()
        {
            // Run this loop while we are connected or if we are still trying to connect.
            while (Status == ClientStatus.Connected || Status == ClientStatus.Connecting)
            {
                // Poll for new events
                NetClient.PollEvents();

                // Wait
                Thread.Sleep(15);
            }
        }

        /// <summary>
        ///     Check if we are still conencted to the server
        /// </summary>
        private void OnPing(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Client not connected, don't worry about this code
            if (Status == ClientStatus.Disconnected)
                return;

            // If we have not heard from the server in TIMEOUT seconds, it's probably gone
            // for now, we just disconnect. In the future we should try reconnecting and
            // displaying a UI.
            if (DateTime.UtcNow - _lastServerPing >= TimeSpan.FromSeconds(TIMEOUT))
            {
                CSM.Log($"Client lost connection with the server (time out, last ping > {TIMEOUT} seconds). Disconnecting...");
                Disconnect();
            }
        }

        /// <summary>
        ///     When we get a message from the server, we handle the message here
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

                // Switch between all the messages
                switch (messageType)
                {
                    case CommandBase.ConnectionResultCommandId:
                        // We only want this message while connecting
                        if (Status != ClientStatus.Connecting)
                            break;

                        // Get the result
                        var connectionResult = CommandBase.Deserialize<ConnectionResultCommand>(message);

                        if (connectionResult.Success)
                        {
                            // Log and set that we are connected.
                            CSM.Log($"Successfully connected to server at {peer.EndPoint.Host}:{peer.EndPoint.Port}.");
                            Status = ClientStatus.Connected;
                        }
                        else
                        {
                            CSM.Log($"Could not connect to server at {peer.EndPoint.Host}:{peer.EndPoint.Port}. Disconnecting... Error Message: {connectionResult.Reason}");
                            ConnectionMessage = $"Could not connect to server at {peer.EndPoint.Host}:{peer.EndPoint.Port}. Disconnecting... Error Message: {connectionResult.Reason}";
                            Disconnect();
                        }
                        break;

                    // Connection close confirmation
                    case CommandBase.ConnectionCloseCommandId:
                        Disconnect();
                        break;

                    case CommandBase.ClientConnectCommandId:
                        var connect = CommandBase.Deserialize<ClientConnectCommand>(message);
                        CSM.Log($"Player {connect.Username} has connected!");
                        MultiplayerManager.Instance.PlayerList.Add(connect.Username);
                        break;

                    case CommandBase.ClientDisconnectCommandId:
                        var disconnect = CommandBase.Deserialize<ClientDisconnectCommand>(message);
                        CSM.Log($"Player {disconnect.Username} has disconnected!");
                        MultiplayerManager.Instance.PlayerList.Remove(disconnect.Username);
                        break;

                    case CommandBase.PlayerListCommand:
                        var list = CommandBase.Deserialize<PlayerListCommand>(message);
                        MultiplayerManager.Instance.PlayerList.Clear();
                        MultiplayerManager.Instance.PlayerList.UnionWith(list.PlayerList);
                        break;

                    // Handle ping commands by returning the ping
                    case CommandBase.PingCommandId:
                        // Update the last server ping
                        _lastServerPing = DateTime.UtcNow;
                        // Send back a ping event
                        peer.Send(ArrayHelpers.PrependByte(CommandBase.PingCommandId, new PingCommand().Serialize()), SendOptions.ReliableOrdered);
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
                        long num = Mathf.Clamp((int)((BuildingRemovedPosition.Position.x / 64f) + 135f), 0, 0x10d); //The buildingID is stored in the M_buildingGrid[index] which is calculated by thís arbitrary calculation using the buildings position
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
                        ushort BuildingId2 = BuildingManager.instance.m_buildingGrid[index2];
                        Singleton<BuildingManager>.instance.RelocateBuilding(BuildingId2, BuildingRelocationData.NewPosition, BuildingRelocationData.Angle);
                        break;

                    case CommandBase.RoadCommandID:
                        UnityEngine.Debug.Log("Road Command Recived");
                        var Roads = CommandBase.Deserialize<RoadCommand>(message);
                        NetInfo netinfo = PrefabCollection<NetInfo>.GetPrefab(Roads.InfoIndex);
                        Singleton<NetManager>.instance.CreateSegment(out ushort id, ref Singleton<SimulationManager>.instance.m_randomizer, netinfo, Roads.StartNode, Roads.EndNode, Roads.StartDirection, Roads.Enddirection, Singleton<SimulationManager>.instance.m_currentBuildIndex, Roads.ModifiedIndex, false);
                        break;
                }
            }
            catch (Exception ex)
            {
                CSM.Log($"Received an error from {peer.EndPoint.Host}:{peer.EndPoint.Port}. Message: {ex.Message}");
            }
        }

        /// <summary>
        ///     Called once we have connected to the server,
        ///     at this point we want to send a connect request packet
        ///     to the server
        /// </summary>
        private void ListenerOnPeerConnectedEvent(NetPeer peer)
        {
            // Build the connection request
            var connectionRequest = new ConnectionRequestCommand
            {
                GameVersion = BuildConfig.applicationVersion,
                ModCount = PluginManager.instance.modCount,
                ModVersion = Assembly.GetAssembly(typeof(Client)).GetName().Version.ToString(),
                Password = _clientConfig.Password,
                Username = _clientConfig.Username
            };

            // Send the message
            peer.Send(ArrayHelpers.PrependByte(CommandBase.ConnectionRequestCommandId, connectionRequest.Serialize()), SendOptions.ReliableOrdered);
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