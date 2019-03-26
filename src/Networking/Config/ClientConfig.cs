namespace CSM.Networking.Config
{
    /// <summary>
    ///     This class contains configuration for the game client.
    /// </summary>
    public class ClientConfig
    {
        /// <summary>
        ///     Creates a new configuration for the game client.
        /// </summary>
        /// <param name="hostAddress">The server address to connect to.</param>
        /// <param name="port">The server port to connect to (defaults to 4230).</param>
        /// <param name="username">The user name to use on this server (defaults to "Tango Player").</param>
        /// <param name="password">The password used to connect to the server (optional)</param>
        /// <param name="requestWorld">The client wants to request the server world.</param>
        public ClientConfig(string hostAddress, int port = 4230, string username = "Tango Player", string password = "", bool requestWorld = false)
        {
            HostAddress = hostAddress;
            Port = port;
            Username = username;
            Password = password;
            RequestWorld = requestWorld;
        }

        /// <summary>
        ///     Gets the server that this client will connect to.
        /// </summary>
        public string HostAddress { get; }

        /// <summary>
        ///     Gets the server port that this client will connect to.
        /// </summary>
        public int Port { get; }

        /// <summary>
        ///     Gets the players user name that they will use
        ///     on the connected server.
        /// </summary>
        public string Username { get; }

        /// <summary>
        ///     Gets the password used to login to the server (if required).
        /// </summary>
        public string Password { get; }

        /// <summary>
        ///     The client wants to request a version of the server world.
        /// </summary>
        public bool RequestWorld { get; }
    }
}