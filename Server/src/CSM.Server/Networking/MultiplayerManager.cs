using ColossalFramework.Threading;
using ColossalFramework.UI;
using CSM.Networking.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CSM.Networking.Status;
using CSM.Server.Util;

namespace CSM.Networking
{
    public class MultiplayerManager
    {
        /// <summary>
        ///     The current player list as server or client.
        /// </summary>
        public HashSet<string> PlayerList { get; } = new HashSet<string>();

        /// <summary>
        ///     The current game server (Use only when this game acts as server!)
        /// </summary>
        public Server CurrentServer { get; } = new Server();

        public bool GameBlocked { get; private set; } = false;

        public void ProcessEvents()
        {
            CurrentServer.ProcessEvents();
        }

        /// <summary>
        ///     Starts the game server on the given port.
        /// </summary>
        /// <param name="config">The ServerConfig</param>
        /// <param name="callback">This callback returns if the server was started successfully.</param>
        public void StartGameServer(ServerConfig config, Action<bool> callback)
        {
            new Thread(() =>
            {
                // Create the server and start it
                bool isConnected = CurrentServer.StartServer(config);
                callback.Invoke(isConnected);
            }).Start();
        }

        /// <summary>
        ///     Stops the client or server, depending on the current role
        /// </summary>
        public void StopEverything()
        {
            CurrentServer.StopServer();
            ChatLogPanel.PrintGameMessage("Server stopped.");
        }

        public bool IsConnected()
        {
            return true;
        }

        public void BlockGame(string JoiningUsername)
        {
            BlockGame(false, JoiningUsername, false);
        }

        private void BlockGame(bool IsSelf, string JoiningUsername, bool IsFirstJoin)
        {
            if (GameBlocked)
                return;
            if (!IsFirstJoin)
                GameBlocked = true;
        }

        public void UnblockGame(bool RemoveFromUI = false)
        {
            if (!GameBlocked)
                return;
            GameBlocked = false;
        }

        private static void QueueMainThread(Action action)
        {
            if (Dispatcher.currentSafe == ThreadHelper.dispatcher)
            {
                action();
            }
            else
            {
                ThreadHelper.dispatcher.Dispatch(action);
            }
        }

        private static MultiplayerManager _multiplayerInstance;
        public static MultiplayerManager Instance => _multiplayerInstance ?? (_multiplayerInstance = new MultiplayerManager());
    }

    /// <summary>
    ///     What state our game is in.
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
        ///     clients and receive information about the game from the clients.
        /// </summary>
        Server
    }
}
