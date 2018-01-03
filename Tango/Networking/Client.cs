using System;
using System.Threading;
using ColossalFramework.Plugins;
using Lidgren.Network;

namespace Tango.Networking
{
    /// <summary>
    ///     Client
    /// </summary>
    public class Client : IDisposable
    {
        #region Private Variables
        // Network
        private NetPeerConfiguration _netPeerConfiguration;
        private NetClient _netClient;

        // Connection
        private string _serverIp;
        private int _serverPort;
        private string _serverPassword;

        // User
        private string _userName;

        // Internal
        private bool _isConnected;
        private bool _isDisposed;

        // Processing
        private ParameterizedThreadStart _pts;
        private Thread _messageProcessingThread;
        #endregion

        #region Getters
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

        #region Constructor
        public Client()
        {
            ResetConfig();

            _netClient = new NetClient(_netPeerConfiguration);

            _pts = ProcessMessage;
            _messageProcessingThread = new Thread(_pts);

        }
        #endregion

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
                    TangoMod.Log(PluginManager.MessageType.Warning, "Could not disconnect old game.");
                    return new ConnectionResult(false, "Could not disconnect old game.");

                }
            }

            _serverIp = ipAddress;
            _serverPort = port;
            _serverPassword = password;

            _userName = username;

            // First message to send to server. The server will read this info and 
            // decide to let us connect or not.
            var approvalMessage = _netClient.CreateMessage();
            approvalMessage.Write("TANGO_CONNECT_REQUEST");
            approvalMessage.Write(_serverPassword);
            approvalMessage.Write(_userName);
            approvalMessage.Write(PluginManager.instance.enabledModCount);

            TangoMod.Log(PluginManager.MessageType.Message, "Client Connecting...");

            try
            {
                // Start the client
                _netClient.Start();

                // Connect to the requested server
                _netClient.Connect(_serverIp, _serverPort, approvalMessage);
            }
            catch (Exception e)
            {
                TangoMod.Log(PluginManager.MessageType.Warning, e.Message);
                return new ConnectionResult(false, e.Message);
            }

            // Is the client connected?
            if (_netClient.ConnectionStatus == NetConnectionStatus.Connected)
            {
                TangoMod.Log(PluginManager.MessageType.Message, "Client Connected");

                _isConnected = true;

                _messageProcessingThread = new Thread(_pts);
                _messageProcessingThread.Start(_netClient);

                return new ConnectionResult(true);
            }

            TangoMod.Log(PluginManager.MessageType.Warning, "Could not connect to server.");
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

            TangoMod.Log(PluginManager.MessageType.Message, "Disconnecting...");

            try
            {
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






        // ----------------------- TO SORT -----------------------

        private void ResetConfig()
        {
            _netPeerConfiguration = new NetPeerConfiguration("Tango")
            {
                MaximumHandshakeAttempts = 1,
                ResendHandshakeInterval = 1,
                AutoFlushSendQueue = false,
                ConnectionTimeout = 30.0f
            };
        }


        private void ProcessMessage(object obj)
        {
            try
            {
                var netClient = (NetClient)obj;
                NetIncomingMessage message;

                TangoMod.Log(PluginManager.MessageType.Message, "Started client processing thread.");

                while (_isConnected)
                {
                    while ((message = netClient.ReadMessage()) != null)
                    {
                        switch (message.MessageType)
                        {
                            // Debug
                            case NetIncomingMessageType.VerboseDebugMessage: 
                            case NetIncomingMessageType.DebugMessage: 
                            case NetIncomingMessageType.WarningMessage: 
                            case NetIncomingMessageType.ErrorMessage: 
                                TangoMod.Log(PluginManager.MessageType.Warning, "Debug Message: " + message.ReadString());
                                break;

                            // Client disconnected or connected
                            case NetIncomingMessageType.StatusChanged:
                                var state = (NetConnectionStatus)message.ReadByte();
                                if (state == NetConnectionStatus.Connected)
                                {
                                    _isConnected = true;
                                }
                                else
                                {
                                    _isConnected = false;
                                }
                                break;

                            case NetIncomingMessageType.Data:
                                var type = message.ReadInt32();

                                TangoMod.Log(PluginManager.MessageType.Message, "Type: " + type);
                                break;

                        }
                    }
                }
            }
            catch (Exception e)
            {
                TangoMod.Log(PluginManager.MessageType.Error, "Client thread crashes: " + e.Message);
            }
            finally
            {
                TangoMod.Log(PluginManager.MessageType.Message, "Client thread stopped.");
            }
        }

      


        #region Setup
        private static Client _serverInstance;
        public static Client Instance => _serverInstance ?? (_serverInstance = new Client());
        #endregion


        public void Dispose()
        {
            Disconnect();

            if (!_isDisposed)
            {
                GC.SuppressFinalize(this);
                _isDisposed = true;
            }
        }

        ~Client()
        {
            Dispose();
        }
    }
}
