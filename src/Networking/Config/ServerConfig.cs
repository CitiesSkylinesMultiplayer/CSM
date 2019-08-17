namespace CSM.Networking.Config
{
    /// <summary>
    ///     This class contains configuration for the game server.
    /// </summary>
    public class ServerConfig
    {
        /// <summary>
        ///      Creates a new configuration for the game server.
        /// </summary>
        /// <param name="port">The port to run this server on (defaults to 4230).</param>
        /// <param name="username">The user name for the hosting player (defaults to "Tango Player")</param>
        /// <param name="password">The optional password for this server.</param>
        /// <param name="maxPlayers">The maximum amount of players that can connect to the server (defaults to 5).</param>
        public ServerConfig(int port = 4230, string username = "Tango Player", string password = "", int maxPlayers = 5)
        {
            Port = port;
            Username = username;
            Password = password;
            MaxPlayers = maxPlayers;
        }

        /// <summary>
        ///     Gets the port that this server will run on.
        /// </summary>
        public int Port { get; }

        /// <summary>
        ///     Gets the maximum amount of players that can connect to this server.
        /// </summary>
        public int MaxPlayers { get; }

        /// <summary>
        ///     Gets the user name for the player hosting the server.
        /// </summary>
        public string Username { get; }

        /// <summary>
        ///     Gets the optional server password.
        /// </summary>
        public string Password { get; }
    }
}
