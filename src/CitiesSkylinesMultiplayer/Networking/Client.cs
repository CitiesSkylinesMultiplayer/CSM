using System;
using System.IO;
using System.Linq;
using System.Threading;
using CitiesSkylinesMultiplayer.Commands;
using ColossalFramework.Plugins;
using LiteNetLib;
using LiteNetLib.Utils;
using ProtoBuf;

namespace CitiesSkylinesMultiplayer.Networking
{
    /// <summary>
    ///     Client
    /// </summary>
    public class Client
    {
        #region Variables
        private static Client _serverInstance;
        public static Client Instance => _serverInstance ?? (_serverInstance = new Client());

        // Network
        private NetManager _netClient;
        private NetPeer _netConnection;
        private EventBasedNetListener _listener;

        // Connection
        private string _serverIp;
        private int _serverPort;
        private string _serverPassword;

        // User
        private string _userName;

        // Internal
        private bool _isConnected;

        /// <summary>
        ///     Is the client currently connected to the server.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                // First we check to see if the actual connection is live
                if (_netClient.ConnectionStatus == NetConnectionStatus.Connected)
                    return true;

                // The connection should be live (could be reconnecting?)
                if (_isConnected)
                    return true;

                // We are not connected
                return false;
            }
        }

        /// <summary>
        ///     Can this client send messages to the server. In this case it can
        ///     only if the client is connected to a server.
        /// </summary>
        public bool CanSendMessage => IsConnected;

        #endregion

        public Client()
        {
            // Set up network items
            _listener = new EventBasedNetListener();
            _netClient = new NetManager(_listener, "Tango");

            // Listen to events
            _listener.NetworkReceiveEvent += ListenerOnNetworkReceiveEvent;
            _listener.NetworkErrorEvent += ListenerOnNetworkErrorEvent;
            _listener.PeerConnectedEvent += ListenerOnPeerConnectedEvent;
        }

        #region Connect
        /// <summary>
        /// Attempt to connect to a server,
        /// </summary>
        /// <param name="ipAddress">Server IP address</param>
        /// <param name="port">Port that the server is running on</param>
        /// <param name="username">Username to use on the server</param>
        /// <param name="password">The server password (if required)</param>
        /// <returns></returns>
        public ConnectionResult Connect(string ipAddress, int port, string username, string password = "")
        {
            if (IsConnected)
            {
                // Disconnect first.
                var disconnectResult = Disconnect();

                // Could not disconnect, so we do not connect
                if (!disconnectResult)
                {
                    CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Warning, "Could not disconnect old game.");
                    return new ConnectionResult(false, "Could not disconnect old game.");

                }
            }

            _serverIp = ipAddress;
            _serverPort = port;
            _serverPassword = password;
            _userName = username;

            CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Message, "Client Connecting...");

            try
            {
                // Start the client
                _netClient.Start();

                // Connect to the requested server
                _netConnection = _netClient.Connect(_serverIp, _serverPort);
            }
            catch (Exception e)
            {
                CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Warning, e.Message);
                return new ConnectionResult(false, e.Message);
            }

            // Is the client connected?
            if (_netConnection.ConnectionState == ConnectionState.Connected)
            {
                CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Message, "Client Connected");

                _isConnected = true;

                return new ConnectionResult(true);
            }

            CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Warning, "Could not connect to server.");
            return new ConnectionResult(false, "Could not connect to server.");
        }
        #endregion

        #region Disconnect
        /// <summary>
        /// Attempt to disconnect from the server
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            // We are not connected, so we are already disconnect :D
            if (!IsConnected)
                return true;

            CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Message, "Disconnecting...");

            try
            {
                _netClient.d



                _netClient.Disconnect("TANGO_DISCONNECT");
            }
            catch
            {
                return false;
            }
            finally
            {
                // Reconfiguration
                ResetConfig();
                _netClient = new NetClient(_netPeerConfiguration);

                _isConnected = false;
            }

            return true;
        }
        #endregion

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
                    // Case 0 is a connection accept
                    case 0:
                        var connectionResult = Commands.ConnectionResult.Deserialize(message);



                        break;
                }


                // TODO Handle the protobug messages, for now just print to console
                CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Message, reader.GetString());

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
                Password = _serverPassword,
                Username = _userName
            };

            // Send the message
            peer.Send(connectionRequest.Serialize(), SendOptions.ReliableOrdered);
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
