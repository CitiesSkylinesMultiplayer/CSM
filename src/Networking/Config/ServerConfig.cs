﻿namespace CSM.Networking.Config
{
    /// <summary>
    ///     This class contains configuration for the game server.
    /// </summary>
    [System.Serializable]
    public class ServerConfig : BaseServerConfig
    {
        /// <summary>
        ///      Creates a new configuration for the game server.
        /// </summary>
        /// <param name="port">The port to run this server on.</param>
        /// <param name="username">The user name for the hosting player.</param>
        /// <param name="password">The optional password for this server.</param>
        /// <param name="maxPlayers">The maximum amount of players that can connect to the server.</param>
        public ServerConfig(int port, string username, string password, int maxPlayers) : base(port, password, maxPlayers)
        {
            Username = username;
        }

        public ServerConfig() : base()
        {
            Username = "";
        }
        /// <summary>
        ///     Gets the user name for the player hosting the server.
        /// </summary>
        public string Username;
    }
}
