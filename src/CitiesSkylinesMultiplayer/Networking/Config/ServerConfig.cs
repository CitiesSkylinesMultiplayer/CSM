namespace CitiesSkylinesMultiplayer.Networking.Config
{
    /// <summary>
    ///     Config for game server
    /// </summary>
    public class ServerConfig
    {
        public ServerConfig(int port = 4230, string username = "Tango Player", string password = "", int maxPlayers = 100)
        {
            Port = port;
            Username = username;
            Password = password;
            MaxPlayers = maxPlayers;
        }

        /// <summary>
        ///     Port to run the server on
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        ///     Max amount of players
        /// </summary>
        public int MaxPlayers { get; set; }

        /// <summary>
        ///     Optional server password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Username for the person hosting the server
        /// </summary>
        public string Username { get; set; }
    }
}