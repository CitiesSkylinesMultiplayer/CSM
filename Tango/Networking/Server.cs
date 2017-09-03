using System;
using Lidgren.Network;

namespace Tango.Networking
{
    public class Server : IDisposable
    {
        #region Setup
        private static Server _serverInstance;
        public static Server Instance => _serverInstance ?? (_serverInstance = new Server());
        #endregion

        private NetServer _netServer;
        private NetPeerConfiguration _natPeerConfiguration;

        private bool _isDisposed;

        /// <summary>
        /// Is the server currently running
        /// </summary>
        public bool IsServerStarted { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="password"></param>
        public void StartServer(int port = 2592, string password = "")
        {
            // Server already started
            if (IsServerStarted)
                return;

            _natPeerConfiguration = new NetPeerConfiguration("Tango")
            {
                Port = port,
                AutoFlushSendQueue = false
            };

            _natPeerConfiguration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            _netServer = new NetServer(_natPeerConfiguration);
            _netServer.Start();
            IsServerStarted = true;
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

            try
            {
                _netServer.Shutdown("disconnect.all");
            }
            finally
            {
                IsServerStarted = false;
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
