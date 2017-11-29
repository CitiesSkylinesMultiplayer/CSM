using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Networking
{
    public class MultiplayerManager
    {
        /// <summary>
        /// 
        /// </summary>
        public MultiplayerRole CurrentRole { get; private set; }

        /// <summary>
        /// The Current game server (warning, this will be null if the game is not a server
        /// </summary>
        public Server CurrentServer { get; } = new Server();

        public Client CurrentClient { get;  } = new Client();







        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool StartGameServer(int port = 4230, string password = "")
        {
            if (CurrentServer.IsServerStarted)
                return true;

            // Create the server and start it
            var isConnected = CurrentServer.StartServer(port, password);

            // Set the current role
            CurrentRole = isConnected ? MultiplayerRole.Server : MultiplayerRole.None;

            return isConnected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool StopGameServer()
        {
           CurrentServer.StopServer();

           CurrentRole = CurrentServer.IsServerStarted ? MultiplayerRole.Server : MultiplayerRole.None;

           return !CurrentServer.IsServerStarted;
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
        /// The game is not connected to a server acting
        /// as a server. In this state we leave all game mechanics
        /// alone.
        /// </summary>
        None,

        /// <summary>
        /// The game is connect to a server and must broadcast
        /// it's update to the server and update internal values
        /// from the server.
        /// </summary>
        Client,

        /// <summary>
        /// The game is acting as a server, it will send out updates to all connected
        /// clients and recieve information about the game from the clients.
        /// </summary>
        Server
    }
}
