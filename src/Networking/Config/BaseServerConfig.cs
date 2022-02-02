namespace CSM.Networking.Config
{
    /// <summary>
    ///     This class contains configuration for the game server.
    /// </summary>
    [System.Serializable]
    public class BaseServerConfig
    {
        /// <summary>
        ///      Creates a new configuration for the game server.
        /// </summary>
        /// <param name="port">The port to run this server on.</param>
        /// <param name="username">The user name for the hosting player.</param>
        /// <param name="password">The optional password for this server.</param>
        /// <param name="maxPlayers">The maximum amount of players that can connect to the server.</param>
        public BaseServerConfig(int port, string password, int maxPlayers)
        {
            Port = port;
            Password = password;
            MaxPlayers = maxPlayers;
        }

        public BaseServerConfig()
        {
            Port = 4230;
            Password = "";
            MaxPlayers = 0;
        }

        /// <summary>
        ///     Gets the port that this server will run on.
        /// </summary>
        public int Port;

        /// <summary>
        ///     Gets the maximum amount of players that can connect to this server.
        /// </summary>
        public int MaxPlayers;

        /// <summary>
        ///     Gets the optional server password.
        /// </summary>
        public string Password;
    }
}
