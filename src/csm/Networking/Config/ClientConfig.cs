using System;

namespace CSM.Networking.Config
{
    /// <summary>
    ///     This class contains configuration for the game client.
    /// </summary>
    [Serializable]
    public class ClientConfig
    {
        /// <summary>
        ///     Creates a new configuration for the game client.
        /// </summary>
        /// <param name="hostAddress">The server address to connect to.</param>
        /// <param name="port">The server port to connect to.</param>
        /// <param name="username">The user name to use on this server.</param>
        /// <param name="password">The password used to connect to the server</param>
        public ClientConfig(string hostAddress, int port, string username, string password)
        {
            HostAddress = hostAddress;
            Port = port;
            Username = username;
            Password = password;
        }

        public ClientConfig()
        {
            HostAddress = "localhost";
            Port = 4230;
            Username = "";
            Password = "";
        }

        /// <summary>
        ///     Gets the server that this client will connect to.
        /// </summary>
        public string HostAddress;

        /// <summary>
        ///     Gets the server port that this client will connect to.
        /// </summary>
        public int Port;

        /// <summary>
        ///     Gets the players user name that they will use
        ///     on the connected server.
        /// </summary>
        public string Username;

        /// <summary>
        ///     Gets the password used to login to the server (if required).
        /// </summary>
        public string Password;
    }
}
