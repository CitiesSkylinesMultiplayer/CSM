using System;

namespace CSM.Networking.Config
{
    /// <summary>
    ///     This class contains configuration for the game server.
    /// </summary>
    [Serializable]
    public class ServerConfig
    {
        /// <summary>
        ///      Creates a new configuration for the game server.
        /// </summary>
        /// <param name="port">The port to run this server on.</param>
        /// <param name="username">The user name for the hosting player.</param>
        /// <param name="password">The optional password for this server.</param>
        /// <param name="maxPlayers">The maximum amount of players that can connect to the server.</param>
        /// <param name="enablePortForwarding">Whether automatic port forwarding (UPnP) should be enabled.</param>
        /// <param name="name">The display name of the server, shown in the public server list.</param>
        /// <param name="listPublicly">Whether this server should be listed in the public server list.</param>
        public ServerConfig(int port, string username, string password, int maxPlayers, bool enablePortForwarding,
            string name = "", bool listPublicly = false)
        {
            Port = port;
            Username = username;
            Password = password;
            MaxPlayers = maxPlayers;
            EnablePortForwarding = enablePortForwarding;
            Name = name;
            ListPublicly = listPublicly;
        }

        public ServerConfig()
        {
            Port = 4230;
            Username = "";
            Password = "";
            MaxPlayers = 0;
            EnablePortForwarding = true;
            Name = "";
            ListPublicly = false;
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
        ///     Gets the user name for the player hosting the server.
        /// </summary>
        public string Username;

        /// <summary>
        ///     Gets the optional server password.
        /// </summary>
        public string Password;

        /// <summary>
        ///     Gets whether automatic port forwarding (UPnP) should be enabled.
        ///     Default is false for security reasons.
        /// </summary>
        public bool EnablePortForwarding;

        /// <summary>
        ///     Gets the display name of the server, shown in the public server list.
        /// </summary>
        public string Name;

        /// <summary>
        ///     Gets whether this server should be listed in the public server list.
        /// </summary>
        public bool ListPublicly;
    }
}
