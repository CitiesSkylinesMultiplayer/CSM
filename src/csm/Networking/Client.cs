using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using ColossalFramework;
using ColossalFramework.Plugins;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Networking;
using CSM.API.Networking.Status;
using CSM.BaseGame.Helpers;
using CSM.Commands;
using CSM.Commands.Data.Internal;
using CSM.Helpers;
using CSM.Mods;
using CSM.Networking.Config;
using CSM.Util;
using LiteNetLib;

namespace CSM.Networking
{
    /// <summary>
    ///     Client
    /// </summary>
    public class Client
    {
        // The client
        private readonly LiteNetLib.NetManager _netClient;
        
        /// <summary>
        ///     Configuration for the client
        /// </summary>
        public ClientConfig Config { get; private set; }

        /// <summary>
        ///     The current status of the client
        /// </summary>
        public ClientStatus Status {
            get => ClientPlayer.Status;
            set => ClientPlayer.Status = value;
        }

        /// <summary>
        ///     The assigned client id.
        /// </summary>
        public int ClientId { get; set; }

        public Player ClientPlayer { get; set; } = new Player();

        /// <summary>
        ///     If the status is disconnected, this will contain
        ///     the reason why.
        /// </summary>
        public string ConnectionMessage { get; set; } = "Unknown error";

        /// <summary>
        ///     If the join game panel should show connection troubleshooting
        ///     information.
        /// </summary>
        public bool ShowTroubleshooting { get; set; } = false;

        private bool _mainMenuEventProcessing = false;

        public Client()
        {
            // Set up network items
            EventBasedNetListener listener = new EventBasedNetListener();
            _netClient = new LiteNetLib.NetManager(listener)
            {
                NatPunchEnabled = true,
                UnconnectedMessagesEnabled = true
            };

            // Listen to events
            listener.NetworkReceiveEvent += ListenerOnNetworkReceiveEvent;
            listener.NetworkErrorEvent += ListenerOnNetworkErrorEvent;
            listener.PeerConnectedEvent += ListenerOnPeerConnectedEvent;
            listener.PeerDisconnectedEvent += ListenerOnPeerDisconnectedEvent;
            listener.NetworkLatencyUpdateEvent += ListenerOnNetworkLatencyUpdateEvent;
        }

        /// <summary>
        ///     Attempt to connect to a server
        /// </summary>
        /// <param name="clientConfig">Client config params</param>
        /// <returns>True if the client is connected to the server, false if not</returns>
        public bool Connect(ClientConfig clientConfig)
        {
            ShowTroubleshooting = false;
            // Let the user know that we are trying to connect to a server
            Log.Info($"Attempting to connect to server at {clientConfig.HostAddress}:{clientConfig.Port}...");

            if (Status != ClientStatus.Disconnected)
            {
                Log.Warn("Current status is not disconnected, ignoring connection attempt.");
                return false;
            }

            Status = ClientStatus.PreConnecting;

            // Set the configuration
            Config = clientConfig;
            ClientPlayer.Username = Config.Username;

            // Start the client, if client setup fails, return out and
            // tell the user
            bool result = _netClient.Start();
            if (!result)
            {
                Log.Error("The client failed to start.");
                ConnectionMessage = "Client failed to start.";
                Disconnect(); // make sure we are fully disconnected
                return false;
            }

            return SetupHolePunching();
        }

        private bool SetupHolePunching()
        {
            // Given string to IP address (resolves domain names).
            IPAddress resolvedAddress;
            try
            {
                resolvedAddress = NetUtils.ResolveAddress(Config.HostAddress);
            }
            catch
            {
                ConnectionMessage = "Invalid server IP";
                Disconnect(); // make sure we are fully disconnected
                return false;
            }

            EventBasedNatPunchListener natPunchListener = new EventBasedNatPunchListener();
            Stopwatch timeoutWatch = new Stopwatch();

            // Callback on for each possible IP address to connect to the server.
            // Can potentially be called multiple times (local and public IP address).
            natPunchListener.NatIntroductionSuccess += (point, type, token) =>
            {
                timeoutWatch.Stop();

                if (Status == ClientStatus.PreConnecting)
                {
                    Log.Info($"Trying endpoint {point} after NAT hole punch...");
                    bool success = DoConnect(point);
                    if (!success)
                    {
                        Status = Status == ClientStatus.Rejected ? ClientStatus.Disconnected : ClientStatus.PreConnecting;
                    }
                }

                timeoutWatch.Start();
            };

            // Register listener and send request to global server
            _netClient.NatPunchModule.Init(natPunchListener);
            _netClient.NatPunchModule.SendNatIntroduceRequest(new IPEndPoint(IpAddress.GetIpv4(CSM.Settings.ApiServer), 4240), $"client_{resolvedAddress}");

            timeoutWatch.Start();
            // Wait for NatPunchModule responses.
            // 5 seconds include only the time waiting for nat punch management.
            // Connection attempts have their own timeout in the DoConnect method
            // The waitWatch is paused during an connection attempt.
            while (Status == ClientStatus.PreConnecting && timeoutWatch.Elapsed < TimeSpan.FromSeconds(5))
            {
                _netClient.NatPunchModule.PollEvents();
                // Wait 50ms
                Thread.Sleep(50);
            }

            if (Status == ClientStatus.PreConnecting) // If timeout, try exact given address
            {
                Log.Info($"No registered server on GS found, trying exact given address {resolvedAddress}:{Config.Port}...");
                bool success = DoConnect(new IPEndPoint(resolvedAddress, Config.Port));
                if (!success)
                {
                    Disconnect(); // Make sure we are fully disconnected
                    return false;
                }

                return true;
            }

            return Status != ClientStatus.Disconnected;
        }

        private bool DoConnect(IPEndPoint point)
        {
            // Try connect to server, update the status to say that
            // we are trying to connect.
            try
            {
                _netClient.Connect(point, "CSM");
            }
            catch (Exception ex)
            {
                ConnectionMessage = "Failed to connect.";
                Log.Error($"Failed to connect to {point.Address}:{point.Port} ", ex);
                ShowTroubleshooting = true;
                return false;
            }

            // Start processing networking
            Status = ClientStatus.Connecting;
            ClientId = 0;

            // We need to wait in a loop for 30 seconds (waiting 500ms each time)
            // while we wait for a successful connection (Status = Connected) or a
            // failed connection (Status = Disconnected).
            Stopwatch waitWatch = new Stopwatch();
            waitWatch.Start();

            // Try connect for 30 seconds
            while (waitWatch.Elapsed < TimeSpan.FromSeconds(10))
            {
                // If we connect, exit the loop and return true
                if (Status == ClientStatus.Connected || Status == ClientStatus.Downloading || Status == ClientStatus.Loading)
                {
                    Log.Info("Client has connected.");
                    return true;
                }

                // The client cannot connect for some reason, the ConnectionMessage
                // variable will contain why.
                if (Status == ClientStatus.Disconnected)
                {
                    Log.Warn("Client disconnected while in connecting loop.");
                    return false;
                }

                if (Status == ClientStatus.Rejected)
                {
                    return false;
                }

                // Wait 250ms
                Thread.Sleep(250);
            }

            // We have timed out
            ConnectionMessage = "Could not connect to server, timed out.";
            Log.Warn("Connection timeout!");
            Status = ClientStatus.PreConnecting;

            return false;
        }

        /// <summary>
        ///     Called when the connection was rejected by the server in the ConnectionResultCommand.
        ///     This also means that the network connection was working properly, so we don't
        ///     need to try any further network endpoints.
        /// </summary>
        public void ConnectRejected()
        {
            Status = ClientStatus.Rejected;
            _netClient.Stop();
            TransactionHandler.ClearTransactions();
        }

        /// <summary>
        ///     Attempt to disconnect from the server
        /// </summary>
        public void Disconnect()
        {
            bool needsUnload = (Status == ClientStatus.Connected);

            // Update status and stop client
            Status = ClientStatus.Disconnected;
            _netClient.Stop();
            MultiplayerManager.Instance.PlayerList.Clear();
            TransactionHandler.ClearTransactions();
            ToolSimulator.Clear();

            if (needsUnload)
            {
                Singleton<SimulationManager>.instance.m_ThreadingWrapper.QueueMainThread(() =>
                {
                    // Go back to the main menu after disconnecting
                    Singleton<LoadingManager>.instance.UnloadLevel();
                });
            }

            Log.Info("Disconnected from server");
        }

        public void SendToServer(CommandBase message)
        {
            if (Status == ClientStatus.Disconnected || Status == ClientStatus.PreConnecting)
            {
                Log.Error("Attempted to send message to server, but the client is not connected");
                return;
            }

            NetPeer server = _netClient.ConnectedPeerList[0];

            Log.Debug($"Sending {message.GetType().Name} to server");

            server.Send(Serializer.Serialize(message), DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        ///     Polls new events from the server.
        /// </summary>
        public void ProcessEvents()
        {
            // Poll for new events
            _netClient.PollEvents();
        }

        /// <summary>
        ///     When we get a message from the server, we handle the message here
        ///     and perform any necessary tasks.
        /// </summary>
        private void ListenerOnNetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            try
            {
                CommandReceiver.Parse(reader, peer);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to handle command from server: ", ex);
            }
        }

        /// <summary>
        ///     Called once we have connected to the server,
        ///     at this point we want to send a connect request packet
        ///     to the server
        /// </summary>
        private void ListenerOnPeerConnectedEvent(NetPeer peer)
        {
            // Get the major.minor version
            Version version = Assembly.GetAssembly(typeof(Client)).GetName().Version;
            string versionString = $"{version.Major}.{version.Minor}";

            // Build the connection request
            ConnectionRequestCommand connectionRequest = new ConnectionRequestCommand
            {
                GameVersion = BuildConfig.applicationVersion,
                ModCount = PluginManager.instance.modCount,
                ModVersion = versionString,
                Password = Config.Password,
                Username = Config.Username,
                ExpansionBitMask = DLCHelper.GetOwnedExpansions(),
                ModderPackBitMask = DLCHelper.GetOwnedModderPacks(),
                Mods = ModSupport.Instance.RequiredModsForSync
            };

            Log.Info("Sending connection request to server...");

            Command.SendToServer(connectionRequest);
        }

        private void ListenerOnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (Status == ClientStatus.Connecting)
            {
                ConnectionMessage = "Failed to connect!";
                ShowTroubleshooting = true;
            }

            // Log the error message
            Log.Info($"Disconnected from server. Message: {disconnectInfo.Reason}");

            // Log the reason to the console if we are not in 'connecting' state
            if (Status == ClientStatus.Connected)
            {
                switch (disconnectInfo.Reason)
                {
                    case DisconnectReason.Timeout:
                        Chat.Instance.PrintGameMessage("Disconnected: Timed out!");
                        break;

                    case DisconnectReason.DisconnectPeerCalled:
                        Chat.Instance.PrintGameMessage("Disconnected!");
                        break;

                    case DisconnectReason.RemoteConnectionClose:
                        Chat.Instance.PrintGameMessage("Disconnected: Server closed!");
                        break;

                    default:
                        Chat.Instance.PrintGameMessage($"Disconnected: Connection lost ({disconnectInfo.Reason})!");
                        break;
                }
            }

            // If we are connected, disconnect
            if (Status == ClientStatus.Downloading || Status == ClientStatus.Loading || Status == ClientStatus.Connected)
                MultiplayerManager.Instance.StopEverything();

            // In the case of ClientStatus.Connecting, this also ends the wait loop
            Status = ClientStatus.Disconnected;
        }

        /// <summary>
        ///     Called whenever an error happens, we
        ///     write it to the log file.
        /// </summary>
        private void ListenerOnNetworkErrorEvent(IPEndPoint endpoint, SocketError socketError)
        {
            Log.Error($"Network error: {socketError}");
        }
        
        private void ListenerOnNetworkLatencyUpdateEvent(NetPeer peer, int latency)
        {
            ClientPlayer.Latency = latency;
        }

        public void StartMainMenuEventProcessor()
        {
            if (_mainMenuEventProcessing) return;
            new Thread(() =>
            {
                _mainMenuEventProcessing = true;
                while (_mainMenuEventProcessing)
                {
                    // The threading extension is not yet loaded when at the main menu, so
                    // process the events and go on
                    ProcessEvents();

                    Thread.Sleep(100);
                }
            }).Start();
        }

        public void StopMainMenuEventProcessor()
        {
            _mainMenuEventProcessing = false;
        }
    }
}
