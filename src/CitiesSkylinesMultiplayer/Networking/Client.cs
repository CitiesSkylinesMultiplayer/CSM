using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using CitiesSkylinesMultiplayer.Commands;
using CitiesSkylinesMultiplayer.Helpers;
using CitiesSkylinesMultiplayer.Networking.Config;
using CitiesSkylinesMultiplayer.Networking.Status;
using LiteNetLib;
using LiteNetLib.Utils;

namespace CitiesSkylinesMultiplayer.Networking
{
    /// <summary>
    ///     Client
    /// </summary>
    public class Client
    {
        // The client
        private readonly LiteNetLib.NetManager _netClient;

        // Run a background processing thread
        private Thread _clientProcessingThread;

        // Config options for server
        private ClientConfig _clientConfig;

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
            _netClient = new LiteNetLib.NetManager(listener, "Tango");

            // Listen to events
            listener.NetworkReceiveEvent += ListenerOnNetworkReceiveEvent;
            listener.NetworkErrorEvent += ListenerOnNetworkErrorEvent;
            listener.PeerConnectedEvent += ListenerOnPeerConnectedEvent;
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
            CitiesSkylinesMultiplayer.Log($"Attempting to connect to server at {_clientConfig.HostAddress}:{_clientConfig.Port}...");

            // Start the client, if client setup fails, return out and 
            // tell the user
            var result = _netClient.Start();
            if (!result)
            {
                CitiesSkylinesMultiplayer.Log("The client failed to start.");
                ConnectionMessage = "The client failed to start.";
                Disconnect(); // make sure we are fully disconnected
                return false;
            }

            // Try connect to server, update the status to say that
            // we are trying to connect.
            _netClient.Connect(_clientConfig.HostAddress, _clientConfig.Port);

            // Start processing networking
            Status = ClientStatus.Connecting;

            // Setup processing thread
            _clientProcessingThread = new Thread(ProcessEvents);
            _clientProcessingThread.Start();
            
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
            CitiesSkylinesMultiplayer.Log("Could not connect to server, timed out.");

            // Did not connect
            Disconnect(); // make sure we are fully disconnected
            return false;
        }

        /// <summary>
        ///     Attempt to disconnect from the server
        /// </summary>
        /// <returns></returns>
        public void Disconnect() 
        {
            // Update status and stop client
            Status = ClientStatus.Disconnected;
            _netClient.Stop();

            CitiesSkylinesMultiplayer.Log("Disconnected from server.");
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
                _netClient.PollEvents();

                // Wait
                Thread.Sleep(15);
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
                    case CommandBase.ConnectionResultCommand:
                        // We only want this message while connecting
                        if (Status != ClientStatus.Connecting)
                            break;

                        // Get the result
                        var connectionResult = ConnectionResult.Deserialize(message);

                        if (connectionResult.Success)
                        {
                            // Log and set that we are connected.
                            CitiesSkylinesMultiplayer.Log($"Successfully connected to server at {peer.EndPoint.Host}:{peer.EndPoint.Port}.");
                            Status = ClientStatus.Connected;
                        }
                        else
                        {
                            CitiesSkylinesMultiplayer.Log($"Could not connect to server at {peer.EndPoint.Host}:{peer.EndPoint.Port}. Disconnecting... Error Message: {connectionResult.Reason}");
                            ConnectionMessage = $"Could not connect to server at {peer.EndPoint.Host}:{peer.EndPoint.Port}. Disconnecting... Error Message: {connectionResult.Reason}";
                            Disconnect();
                        }
                        break;
                    // Handle ping commands by returning the ping
                    case CommandBase.PingCommand:
                        peer.Send(ArrayHelpers.PrependByte(CommandBase.PingCommand, new Ping().Serialize()), SendOptions.ReliableOrdered);
                        break;
                }
            }
            catch (Exception ex)
            {
                CitiesSkylinesMultiplayer.Log($"Received an error from {peer.EndPoint.Host}:{peer.EndPoint.Port}. Message: {ex.Message}");
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
            var connectionRequest = new ConnectionRequest
            {
                GameVersion = "1.0.0",
                ModCount = 1,
                ModVersion = "1.0.0",
                Password = _clientConfig.Password,
                Username = _clientConfig.Username
            };

            // Send the message
            peer.Send(ArrayHelpers.PrependByte(CommandBase.ConnectionRequestCommand, connectionRequest.Serialize()), SendOptions.ReliableOrdered);
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