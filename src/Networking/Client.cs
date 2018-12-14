using ColossalFramework.Plugins;
using CSM.Commands;
using CSM.Helpers;
using CSM.Networking.Config;
using CSM.Networking.Status;
using CSM.Panels;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Diagnostics;
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

        // Class logger
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Configuration for the client
        /// </summary>
        public ClientConfig Config { get; private set; }

        /// <summary>
        ///     The current status of the client
        /// </summary>
        public ClientStatus Status { get; set; }

        /// <summary>
        ///     If the status is disconnected, this will contain
        ///     the reason why.
        /// </summary>
        public string ConnectionMessage { get; set; } = "Unknown error";

        public Client()
        {
            // Set up network items
            var listener = new EventBasedNetListener();
            _netClient = new LiteNetLib.NetManager(listener, "Cities: Skylines Multiplayer");

            // Listen to events
            listener.NetworkReceiveEvent += ListenerOnNetworkReceiveEvent;
            listener.NetworkErrorEvent += ListenerOnNetworkErrorEvent;
            listener.PeerConnectedEvent += ListenerOnPeerConnectedEvent;
            listener.PeerDisconnectedEvent += ListenerOnPeerDisconnectedEvent;
        }

        /// <summary>
        ///     Attempt to connect to a server
        /// </summary>
        /// <param name="clientConfig">Client config params</param>
        /// <returns>True if the client is connected to the server, false if not</returns>
        public bool Connect(ClientConfig clientConfig)
        {
            // Let the user know that we are trying to connect to a server
            _logger.Info($"Attempting to connect to server at {clientConfig.HostAddress}:{clientConfig.Port}...");
            ChatLogPanel.GetDefault().AddGameMessage(ChatLogPanel.MessageType.Normal, $"Attempting to connect to server at {clientConfig.HostAddress}:{clientConfig.Port}...");

            // if we are currently trying to connect, cancel
            // and try again.
            if (Status == ClientStatus.Connecting)
            {
                _logger.Info("Current status is 'connecting', attempting to disconnect first.");
                Disconnect();
            }

            // The client is already connected so we need to
            // disconnect.
            if (Status == ClientStatus.Connected)
            {
                _logger.Info("Current status is 'connected', attempting to disconnect first.");
                Disconnect();
            }

            // Set the configuration
            Config = clientConfig;

            // Start the client, if client setup fails, return out and
            // tell the user
            var result = _netClient.Start();
            if (!result)
            {
                _logger.Error("The client failed to start.");
                ConnectionMessage = "The client failed to start.";
                Disconnect(); // make sure we are fully disconnected
                return false;
            }

            _logger.Info("Set status to 'connecting'...");

            // Try connect to server, update the status to say that
            // we are trying to connect.
            _netClient.Connect(Config.HostAddress, Config.Port);

            // Start processing networking
            Status = ClientStatus.Connecting;

            // We need to wait in a loop for 30 seconds (waiting 500ms each time)
            // while we wait for a successful connection (Status = Connected) or a
            // failed connection (Status = Disconnected).
            var waitWatch = new Stopwatch();
            waitWatch.Start();

            // Try connect for 30 seconds
            while (waitWatch.Elapsed < TimeSpan.FromSeconds(30))
            {
                // If we connect, exit the loop and return true
                if (Status == ClientStatus.Connected)
                {
                    _logger.Info("Client has connected.");
                    return true;
                }

                // The client cannot connect for some reason, the ConnectionMessage
                // variable will contain why.
                if (Status == ClientStatus.Disconnected)
                {
                    _logger.Error("Client disconnected while in connecting loop.");
                    Disconnect(); // make sure we are fully disconnected
                    return false;
                }

                // Wait 500ms
                Thread.Sleep(500);
            }

            // We have timed out
            ConnectionMessage = "Could not connect to server, timed out.";
            _logger.Error("Connection timeout!");

            // Did not connect
            Disconnect(); // make sure we are fully disconnected
            return false;
        }

        /// <summary>
        ///     Attempt to disconnect from the server
        /// </summary>
        public void Disconnect()
        {
            _logger.Info("Disconnecting from server...");
            // Update status and stop client
            Status = ClientStatus.Disconnected;
            _netClient.Stop();
        }

        public void SendToServer(byte messageId, CommandBase message)
        {
            if (Status == ClientStatus.Disconnected)
            {
                _logger.Error("Attempted to send message to server, but the client is disconnected");
                return;
            }

            var server = _netClient.GetFirstPeer();

            _logger.Info($"Sending message id of {messageId} to server at {server.EndPoint.Host}:{server.EndPoint.Port}");
            _logger.Debug(message.Serialize());

            server.Send(ArrayHelpers.PrependByte(messageId, message.Serialize()), SendOptions.ReliableOrdered);
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
        private void ListenerOnNetworkReceiveEvent(NetPeer peer, NetDataReader reader)
        {
            try
            {
                Command.ParseOnClient(reader.Data);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Encountered an error while reading command from {peer.EndPoint.Host}:{peer.EndPoint.Port}:");
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
                Password = Config.Password,
                Username = Config.Username
            };

            _logger.Info("Sending connection request to server...");

            Command.SendToServer(connectionRequest);
        }

        private void ListenerOnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (Status == ClientStatus.Connecting)
                ConnectionMessage = $"Failed to connect! ({disconnectInfo.Reason})";

            // Log the error message
            _logger.Info($"Disconnected from server. Message: {disconnectInfo.Reason}, Code: {disconnectInfo.SocketErrorCode}");

            // Log the reason to the console
            switch (disconnectInfo.Reason)
            {
                case DisconnectReason.Timeout:
                    ChatLogPanel.GetDefault().AddGameMessage(ChatLogPanel.MessageType.Normal, "Disconnected: Timed out!");
                    break;

                case DisconnectReason.DisconnectPeerCalled:
                    ChatLogPanel.GetDefault().AddGameMessage(ChatLogPanel.MessageType.Normal, "Disconnected!");
                    break;

                case DisconnectReason.RemoteConnectionClose:
                    ChatLogPanel.GetDefault().AddGameMessage(ChatLogPanel.MessageType.Normal, "Disconnected: Server closed!");
                    break;

                default:
                    ChatLogPanel.GetDefault().AddGameMessage(ChatLogPanel.MessageType.Normal, $"Disconnected: Connection lost ({disconnectInfo.Reason})!");
                    break;
            }

            // If we are connected, disconnect
            if (Status == ClientStatus.Connected)
                MultiplayerManager.Instance.StopEverything();

            // In the case of ClientStatus.Connecting, this also ends the wait loop
            Status = ClientStatus.Disconnected;
        }

        /// <summary>
        ///     Called whenever an error happens, we
        ///     log this to the console for now.
        /// </summary>
        private void ListenerOnNetworkErrorEvent(NetEndPoint endpoint, int socketerrorcode)
        {
            ChatLogPanel.GetDefault().AddGameMessage(ChatLogPanel.MessageType.Error, $"[{endpoint.Host}:{endpoint.Port}] is causing errors. See log.");
            _logger.Error($"Received an error from {endpoint.Host}:{endpoint.Port}. Code: {socketerrorcode}");
        }
    }
}