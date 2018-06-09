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
    ///     Server
    /// </summary>
    public class Server
    {
        // The server
        private readonly LiteNetLib.NetManager _netServer;

        // Run a background processing thread
        private readonly Thread _serverProcessingThread;

        // Config options for server
        private ServerConfig _serverConfig;

        /// <summary>
        ///     Is the server currently running
        /// </summary>
        public bool IsServerRunning { get; private set; }

        public Server()
        {
            // Set up network items
            var listener = new EventBasedNetListener();
            _netServer = new LiteNetLib.NetManager(listener, "Tango");

            // Listen to events
            listener.NetworkReceiveEvent += ListenerOnNetworkReceiveEvent;
            listener.NetworkErrorEvent += ListenerOnNetworkErrorEvent;

            // Set up processing thread
            _serverProcessingThread = new Thread(ProcessEvents);
        }

        /// <summary>
        ///     Starts the server with the specified config options
        /// </summary>
        /// <param name="serverConfig">Server config information</param>
        /// <returns>If the server has started.</returns>
        public bool StartServer(ServerConfig serverConfig)
        {
            // Server already started
            if (IsServerRunning)
                return true;

            // Set the config
            _serverConfig = serverConfig;

            // Let the user know that we are trying to start the server
            CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Message, $"Attempting to start server on port {_serverConfig.Port}...");

            // Attempt to start the server
            var result = _netServer.Start(_serverConfig.Port);

            // If the server has not started, tell the user and return false.
            if (!result)
            {
                CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Message, "The server failed to start.");
                return false;
            }

            // Start the processing thread
            IsServerRunning = true;
            _serverProcessingThread.Start();

            // Update the console to let the user know the server is running
            CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Message, "The server has started.");
            return true;
        }

        /// <summary>
        ///     Stops the server
        /// </summary>
        public void StopServer()
        {
            // Only shutdown server if it is
            // all ready running.
            if (!IsServerRunning)
                return;

            // Log that the server is stopping
            CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Message, "Stopping server...");

            // Quit out the loop and stop the server
            IsServerRunning = false;
            _netServer.Stop();
        }

        /// <summary>
        ///     Runs in the background of the game (another thread), polls for new updates
        ///     from the clients.
        /// </summary>
        private void ProcessEvents()
        {
            while (IsServerRunning)
            {
                // Poll for new events
                _netServer.PollEvents();

                // Wait
                Thread.Sleep(15);
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
                    // Case 0 is a connection request
                    case 0:
                        CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Message, $"Connection request from {peer.EndPoint.Host}:{peer.EndPoint.Port}.");
                        var connectionResult = Commands.ConnectionRequest.Deserialize(message);

                        // TODO, check these values, but for now, just accept the request.
                        peer.Send(ArrayHelpers.PrependByte(CommandBase.ConnectionResultCommand, new ConnectionResult { Success = true}.Serialize()), SendOptions.ReliableOrdered);
                        break;
                }
            }
            catch (Exception ex)
            {
                CitiesSkylinesMultiplayer.Log(PluginManager.MessageType.Error, $"Received an error from {peer.EndPoint.Host}:{peer.EndPoint.Port}. Message: {ex.Message}");
            }
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