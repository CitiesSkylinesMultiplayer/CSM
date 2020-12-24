using CSM.Networking;
using CSM.Networking.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CSM.StandaloneServer.Networking
{
    public class Host
    {
        /// <summary>
        ///     The current player list as server or client.
        /// </summary>
        public HashSet<string> PlayerList { get; } = new HashSet<string>();

        /// <summary>
        ///     The current game server (Use only when this game acts as server!)
        /// </summary>
        private Server CurrentServer { get; } = new Server();

        public void ProcessEvents()
        {
            CurrentServer.ProcessEvents();
        }

        /// <summary>
        ///     Starts the game server on the given port.
        /// </summary>
        /// <param name="config">The ServerConfig</param>
        /// <param name="callback">This callback returns if the server was started successfully.</param>
        public void StartGameServer(ServerConfig config, Action callback)
        {
            new Thread(() =>
            {
                // Create the server and start it
                CurrentServer.StartServer(config);

                callback();
            }).Start();
        }

        /// <summary>
        ///     Stops the client or server, depending on the current role
        /// </summary>
        public void StopEverything()
        {
            CurrentServer.StopServer();
        }

        private static Host _multiplayerInstance;
        public static Host Instance => _multiplayerInstance ?? (_multiplayerInstance = new Host());
    }
}
