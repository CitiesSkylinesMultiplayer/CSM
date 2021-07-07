using ColossalFramework;
using ColossalFramework.Plugins;
using CSM.Commands;
using CSM.Commands.Data.Internal;
using CSM.Networking.Config;
using CSM.Networking.Status;
using CSM.Server.Util;
using LiteNetLib;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

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

        private bool MainMenuEventProcessing = false;

        public Client()
        {
            // Set up network items
            EventBasedNetListener listener = new EventBasedNetListener();
            _netClient = new LiteNetLib.NetManager(listener);

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
            // Let the user know that we are trying to connect to a server
            Log.Info($"Attempting to connect to server at {clientConfig.HostAddress}:{clientConfig.Port}...");

            // if we are currently trying to connect, cancel
            // and try again.
            if (Status == ClientStatus.Connecting)
            {
                Log.Info("Current status is 'connecting', attempting to disconnect first.");
                Disconnect();
            }

            // The client is already connected so we need to
            // disconnect.
            if (Status == ClientStatus.Connected)
            {
                Log.Info("Current status is 'connected', attempting to disconnect first.");
                Disconnect();
            }

            // Set the configuration
            Config = clientConfig;
            ClientPlayer.Username = Config.Username;

            // Start the client, if client setup fails, return out and
            // tell the user
            bool result = _netClient.Start();
            if (!result)
            {
                Log.Error("The client failed to start.");
                ConnectionMessage = "The client failed to start.";
                Disconnect(); // make sure we are fully disconnected
                return false;
            }

            Log.Info("Set status to 'connecting'...");

            // Try connect to server, update the status to say that
            // we are trying to connect.
            try
            {
                _netClient.Connect(Config.HostAddress, Config.Port, "CSM");
            }
            catch (Exception ex)
            {
                ConnectionMessage = "Failed to connect.";
                Log.Error($"Failed to connect to {Config.HostAddress}:{Config.Port}", ex);
                ChatLogPanel.PrintGameMessage(ChatLogPanel.MessageType.Error, $"Failed to connect: {ex.Message}");
                Disconnect();
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
            while (waitWatch.Elapsed < TimeSpan.FromSeconds(30))
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
                    Disconnect(); // make sure we are fully disconnected
                    return false;
                }

                // Wait 250ms
                Thread.Sleep(250);
            }

            // We have timed out
            ConnectionMessage = "Could not connect to server, timed out.";
            Log.Warn("Connection timeout!");

            // Did not connect
            Disconnect(); // make sure we are fully disconnected
            return false;
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
            if (Status == ClientStatus.Disconnected)
            {
                Log.Error("Attempted to send message to server, but the client is disconnected");
                return;
            }

            NetPeer server = _netClient.ConnectedPeerList[0];

            Log.Debug($"Sending {message.GetType().Name} to server");

            server.Send(message.Serialize(), DeliveryMethod.ReliableOrdered);
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
                Log.Error($"Encountered an error while reading command from {peer.EndPoint.Address}:{peer.EndPoint.Port}:", ex);
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
                DLCBitMask = DLCHelper.GetOwnedDLCs()
            };

            Log.Info("Sending connection request to server...");

            Command.SendToServer(connectionRequest);
        }

        private void ListenerOnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (Status == ClientStatus.Connecting)
                ConnectionMessage = "Failed to connect!";

            // Log the error message
            Log.Info($"Disconnected from server. Message: {disconnectInfo.Reason}, Code: {disconnectInfo.SocketErrorCode}");

            // Log the reason to the console if we are not in 'connecting' state
            if (Status == ClientStatus.Connected)
            {
                switch (disconnectInfo.Reason)
                {
                    case DisconnectReason.Timeout:
                        ChatLogPanel.PrintGameMessage("Disconnected: Timed out!");
                        break;

                    case DisconnectReason.DisconnectPeerCalled:
                        ChatLogPanel.PrintGameMessage("Disconnected!");
                        break;

                    case DisconnectReason.RemoteConnectionClose:
                        ChatLogPanel.PrintGameMessage("Disconnected: Server closed!");
                        break;

                    default:
                        ChatLogPanel.PrintGameMessage($"Disconnected: Connection lost ({disconnectInfo.Reason})!");
                        break;
                }
            }

            // If we are connected, disconnect
            if (Status != ClientStatus.Disconnected && Status != ClientStatus.Connecting)
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
            Log.Error($"Received an error from {endpoint.Address}:{endpoint.Port}. Code: {socketError}");
        }
        
        private void ListenerOnNetworkLatencyUpdateEvent(NetPeer peer, int latency)
        {
            ClientPlayer.Latency = latency;
        }

        public void StartMainMenuEventProcessor()
        {
            if (MainMenuEventProcessing) return;
            new Thread(() =>
            {
                MainMenuEventProcessing = true;
                while (MainMenuEventProcessing)
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
            MainMenuEventProcessing = false;
        }
    }
}
