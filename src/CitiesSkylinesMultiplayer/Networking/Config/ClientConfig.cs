namespace CitiesSkylinesMultiplayer.Networking.Config
{
    /// <summary>
    ///     Config for game client
    /// </summary>
    public class ClientConfig
    {
        public ClientConfig(string hostAddress, int port = 4230, string username = "Tango Player", string password = "")
        {
            HostAddress = hostAddress;
            Port = port;
            Username = username;
            Password = password;
        }

        /// <summary>
        ///     Server address to connect to
        /// </summary>
        public string HostAddress { get; set; }

        /// <summary>
        ///     Server port to conenct to
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        ///     Password for server (if required)
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Players username
        /// </summary>
        public string Username { get; set; }
    }
}
