using CSM.Networking.Config;
using CSM.Panels;
using System;
using System.Collections.Generic;
using System.Threading;
using CSM.Localisation;

namespace CSM.Networking
{
    public class MultiplayerManager
    {
        /// <summary>
        /// The current player list as server or client.
        /// </summary>
        public HashSet<string> PlayerList { get; } = new HashSet<string>();

        /// <summary>
        /// The current role of the game.
        /// </summary>
        public MultiplayerRole CurrentRole { get; private set; }

        /// <summary>
        /// The current game server (Use only when this game acts as server!)
        /// </summary>
        public Server CurrentServer { get; } = new Server();

        /// <summary>
        /// The current client (Use only when this game acts as client!)
        /// </summary>
        public Client CurrentClient { get; } = new Client();

        public void ProcessEvents()
        {
            switch (CurrentRole)
            {
                case MultiplayerRole.Client:
                    CurrentClient.ProcessEvents();
                    break;

                case MultiplayerRole.Server:
                    CurrentServer.ProcessEvents();
                    break;

                case MultiplayerRole.None:
                    if (CurrentClient.Status == Status.ClientStatus.Connecting)
                    {
                        CurrentClient.ProcessEvents();
                    }
                    break;
            }
        }

        /// <summary>
        /// Starts the client and tries to connect to the given server.
        /// </summary>
        /// <param name="ipAddress">The server ip address.</param>
        /// <param name="port">The server port.</param>
        /// <param name="username">The username to connect with.</param>
        /// <param name="password">The password to use.</param>
        /// <param name="requestWorld">The client wants to load the world.</param>
        /// <param name="callback">This callback returns if the connection was successful.</param>
        public void ConnectToServer(string ipAddress, int port, string username, string password, bool requestWorld, Action<bool> callback)
        {
            if (CurrentRole == MultiplayerRole.Server)
            {
                callback.Invoke(false);
                return;
            }

            new Thread(() =>
            {
                // Try connect
                var isConnected = CurrentClient.Connect(new ClientConfig(ipAddress, port, username, password, requestWorld));

                // Set the current role
                CurrentRole = isConnected ? MultiplayerRole.Client : MultiplayerRole.None;

                // Return the status
                callback.Invoke(isConnected);
            }).Start();
        }

        /// <summary>
        /// Starts the game server on the given port.
        /// </summary>
        /// <param name="port">The port to start the server on.</param>
        /// <param name="password">The password to use.</param>
        /// <param name="hostUsername">The username of the host player.</param>
        /// <param name="callback">This callback returns if the server was started successfully.</param>
        public void StartGameServer(int port, string password, string hostUsername, Action<bool> callback)
        {
            if (CurrentRole == MultiplayerRole.Client)
            {
                callback.Invoke(false);
                return;
            }

            new Thread(() =>
            {
                // Create the server and start it
                var isConnected = CurrentServer.StartServer(new ServerConfig(port, hostUsername, password));

                // Set the current role
                CurrentRole = isConnected ? MultiplayerRole.Server : MultiplayerRole.None;

                callback.Invoke(isConnected);
            }).Start();
        }

        /// <summary>
        /// Stops the client or server, depending on the current role
        /// </summary>
        public void StopEverything()
        {
            switch (CurrentRole)
            {
                case MultiplayerRole.Client:
                    CurrentClient.Disconnect();
                    ChatLogPanel.PrintGameMessage($"{Translation.PullTranslation("DisconnectedFromServer")}.");
                    break;

                case MultiplayerRole.Server:
                    CurrentServer.StopServer();
                    ChatLogPanel.PrintGameMessage($"{Translation.PullTranslation("ServerStopped")}.");
                    break;
            }
            CurrentRole = MultiplayerRole.None;
        }

        private static MultiplayerManager _multiplayerInstance;
        public static MultiplayerManager Instance => _multiplayerInstance ?? (_multiplayerInstance = new MultiplayerManager());
    }

    /// <summary>
    /// What state our game is in.
    /// </summary>
    public enum MultiplayerRole
    {
        /// <summary>
        ///     The game is not connected to a server acting
        ///     as a server. In this state we leave all game mechanics
        ///     alone.
        /// </summary>
        None,

        /// <summary>
        ///     The game is connect to a server and must broadcast
        ///     it's update to the server and update internal values
        ///     from the server.
        /// </summary>
        Client,

        /// <summary>
        ///     The game is acting as a server, it will send out updates to all connected
        ///     clients and recieve information about the game from the clients.
        /// </summary>
        Server
    }
}