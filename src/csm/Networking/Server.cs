using System;
using System.Linq;
using System.Threading;
using CSM.Commands;
using CSM.Helpers;
using CSM.Networking.Config;
using CSM.Networking.Status;
using LiteNetLib;
using LiteNetLib.Utils;

namespace CSM.Networking
{
    /// <summary>
    ///     Server
    /// </summary>
    public class Server
    {
        // The server
        private readonly LiteNetLib.NetManager _netServer;

        // Run a background processing thread
        private Thread _serverProcessingThread;

        // Config options for server
        private ServerConfig _serverConfig;

        // Timer for handling ping
        private System.Timers.Timer _pingTimer;

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

            // Setup timer
            _pingTimer = new System.Timers.Timer();
            _pingTimer.Elapsed += OnPing;
            _pingTimer.Interval = 100;
            _pingTimer.Start();
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
            _serverConfig = serverConfig;

            // Let the user know that we are trying to start the server
            CitiesSkylinesMultiplayer.Log($"Attempting to start server on port {_serverConfig.Port}...");

            // Attempt to start the server
            _netServer.DiscoveryEnabled = true;
            var result = _netServer.Start(_serverConfig.Port);

            // If the server has not started, tell the user and return false.
            if (!result)
            {
                CitiesSkylinesMultiplayer.Log("The server failed to start.");
                StopServer(); // Make sure the server is fully stopped
                return false;
            }

            // Update the status
            Status = ServerStatus.Running;

            // Set up processing thread
            _serverProcessingThread = new Thread(ProcessEvents);
            _serverProcessingThread.Start();

            // Update the console to let the user know the server is running
            CitiesSkylinesMultiplayer.Log("The server has started.");
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

            CitiesSkylinesMultiplayer.Log("Stopped server");
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
                _netServer.PollEvents();

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

            // Loop though all connected peers
            foreach (var netPeer in _netServer.GetPeers())
            {
                // Send a ping
                netPeer.Send(ArrayHelpers.PrependByte(CommandBase.PingCommand, new Ping().Serialize()), SendOptions.ReliableOrdered);
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

                // Switch between all the messages
                switch (messageType)
                {
                    case CommandBase.ConnectionRequestCommand:
                        CitiesSkylinesMultiplayer.Log($"Connection request from {peer.EndPoint.Host}:{peer.EndPoint.Port}.");
                        var connectionResult = Commands.ConnectionRequest.Deserialize(message);

                        // TODO, check these values, but for now, just accept the request.
                        peer.Send(ArrayHelpers.PrependByte(CommandBase.ConnectionResultCommand, new ConnectionResult { Success = true}.Serialize()), SendOptions.ReliableOrdered);
                        break;
                }
            }
            catch (Exception ex)
            {
                CitiesSkylinesMultiplayer.Log($"Received an error from {peer.EndPoint.Host}:{peer.EndPoint.Port}. Message: {ex.Message}");
            }
        }

        /// <summary>
        ///     Called whenever an error happens, we
        ///     log this to the console for now.
        /// </summary>
        private void ListenerOnNetworkErrorEvent(NetEndPoint endpoint, int socketerrorcode)
        {
            CitiesSkylinesMultiplayer.Log($"Received an error from {endpoint.Host}:{endpoint.Port}. Code: {socketerrorcode}");
        }
    }
}