using System;
using Lidgren.Network;

namespace Tango.Networking
{
    public class Server : IDisposable
    {
        #region Setup
        private static readonly Lazy<Server> InstanceHolder =
            new Lazy<Server>(() => new Server());

        public static Server Instance => InstanceHolder.Value;
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

            Logger.Info($"Starting server on port: {port}...");

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

            Logger.Info("Stopping Server...");

            try
            {
                _netServer.Shutdown("disconnect.all");
            }
            catch (Exception ex)
            {
                Logger.Fatal("Error stopping server", ex);

                throw;
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
