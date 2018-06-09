using System;
using System.Linq;
using System.Threading;
using CitiesSkylinesMultiplayer.Commands;
using CitiesSkylinesMultiplayer.Helpers;
using CitiesSkylinesMultiplayer.Networking.Config;
using ColossalFramework.Plugins;
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
        private NetPeer _netPeer;

        // Run a background processing thread
        private readonly Thread _clientProcessingThread;

        // Config options for server
        private ClientConfig _clientConfig;

        /// <summary>
        ///     Are we connected to a server
        /// </summary>
        public bool IsClientConnected { get; private set; }

        public Client()
        {
            // Set up network items
            var listener = new EventBasedNetListener();
            _netClient = new LiteNetLib.NetManager(listener, "Tango");

            // Listen to events
            listener.NetworkReceiveEvent += ListenerOnNetworkReceiveEvent;
            listener.NetworkErrorEvent += ListenerOnNetworkErrorEvent;
            listener.PeerConnectedEvent += ListenerOnPeerConnectedEvent;

            // Set up processing thread
            _clientProcessingThread = new Thread(ProcessEvents);
        }

        /// <summary>
        ///     Attempt to connect to a server
        /// </summary>
        /// <param name="clientConfig">Client config params</param>
        /// <returns>True is connected (may need to change, as that's hard to tell</returns>
        public bool Connect(ClientConfig clientConfig)
        {
            if (IsClientConnected)
            {
                // Disconnect first.
                var disconnectResult = Disconnect();

                // Could not disconnect, so we do not connect
                if (!disconnectResult)
                {
                    CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Warning, "Could not disconnect old game.");
                    return false;
                }
            }

            // Set the config
            _clientConfig = clientConfig;

            // Let the user know that we are trying to connect to a server
            CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Message, $"Attempting to connect to server at {_clientConfig.HostAddress}:{_clientConfig.Port}...");

            // Start the client, if client setup fails, return out and 
            // tell the user
            var result = _netClient.Start();
            if (!result)
            {
                CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Warning, "The client failed to start.");
                return false;
            }

            // Try connect to server
            _netPeer = _netClient.Connect(_clientConfig.HostAddress, _clientConfig.Port);

            // Start the processing thread
            IsClientConnected = true;
            _clientProcessingThread.Start();
       
            // Let the user know we connected?
            CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Warning, "Could not connect to server.");
            return true;
        }

        /// <summary>
        /// Attempt to disconnect from the server
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            // We are not connected, so we are already disconnect :D
            if (!IsClientConnected)
                return true;

            CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Message, "Disconnecting...");

            _netClient.Stop();
            IsClientConnected = false;

            return true;
        }

        /// <summary>
        ///     Runs in the background of the game (another thread), polls for new updates
        ///     from the server.
        /// </summary>
        private void ProcessEvents()
        {
            while (IsClientConnected)
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
                    // Case 0 is a connection result
                    case CommandBase.ConnectionResultCommand:
                        var connectionResult = Commands.ConnectionResult.Deserialize(message);
                        break;
                }
            }
            catch (Exception ex)
            {
                CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Error, $"Received an error from {peer.EndPoint.Host}:{peer.EndPoint.Port}. Message: {ex.Message}");
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
            CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Error, $"Received an error from {endpoint.Host}:{endpoint.Port}. Code: {socketerrorcode}");
        }
    }
}