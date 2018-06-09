using System;
using System.Threading;
using ColossalFramework.Plugins;

namespace CitiesSkylinesMultiplayer.Networking
{
    public class Server : IDisposable
    {
        private NetServer _netServer;
        private NetPeerConfiguration _natPeerConfiguration;
        private ParameterizedThreadStart _pts;
        private Thread _messageProcessingThread;
        private bool _isDisposed;

        private int _port;

        /// <summary>
        /// Is the server currently running
        /// </summary>
        public bool IsServerStarted { get; private set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public Server()
        {
            _pts = ProcessMessage;
            _messageProcessingThread = new Thread(_pts); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="password"></param>
        public bool StartServer(int port = 4230, string password = "")
        {
            // Server already started
            if (IsServerStarted)
                return true;

            _port = port;

            CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Message, $"Starting server on port {_port}...");

            _natPeerConfiguration = new NetPeerConfiguration("Tango")
            {
                Port = _port,
                AutoFlushSendQueue = false,
                ConnectionTimeout = 5,
                AcceptIncomingConnections = true
            };

            _natPeerConfiguration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            _netServer = new NetServer(_natPeerConfiguration);
            _netServer.Start();

            if (_netServer.Status == NetPeerStatus.Running)
            {
                IsServerStarted = true;

                _messageProcessingThread = new Thread(_pts);
                _messageProcessingThread.Start(_netServer);

                CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Message, "Server started.");
                return true;
            }

            CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Message, "Server not started...");
            return false;
        }


        /// <summary>
        /// Stop the server
        /// </summary>
        public void StopServer()
        {
            // Only shutdown server if it is
            // all ready running.
            if (!IsServerStarted)
                return;

            CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Message, "Stopping server...");

            try
            {
                _netServer.Shutdown("TANGO_DISCONNECT");
            }
            finally
            {
                IsServerStarted = false;
            }
        }

        private void ProcessMessage(object obj)
        {
            try
            {
                var netServer = (NetServer)obj;
                NetIncomingMessage message;

                CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Message, "Started server processing thread.");

                while (IsServerStarted)
                {
                    while ((message = netServer.ReadMessage()) != null)
                    {
                        switch (message.MessageType)
                        {
                            // Debug
                            case NetIncomingMessageType.VerboseDebugMessage: 
                            case NetIncomingMessageType.DebugMessage: 
                            case NetIncomingMessageType.WarningMessage: 
                            case NetIncomingMessageType.ErrorMessage: 
                                CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Warning, "Debug Message: " + message.ReadString());
                                break;

                            case NetIncomingMessageType.ConnectionApproval:
                                message.SenderConnection.Approve();
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Error, "Server thread crashes: " + e.Message);
            }
            finally
            {
                CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Message, "Server thread stopped.");
            }
        }

        public void Dispose()
        {
            StopServer();

            if (!_isDisposed)
            {
                GC.SuppressFinalize(this);
                _isDisposed = true;
            }
        }

        ~Server()
        {
            Dispose();
        }
    }
}
