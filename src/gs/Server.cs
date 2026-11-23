using System;
using System.Net;

namespace CSM.GS
{
    /// <summary>
    ///     Represents a game server connected to the current CSM GS.
    ///     As long as the game server is running, this object will stay
    ///     in memory.
    /// </summary>
    public class Server
    {
        /// <summary>
        ///     A unique token identifying this server (so the public IP address does
        ///     not need to be displayed)
        /// </summary>
        public string Token { get; }

        /// <summary>
        ///     The internal IP address (behind the NAT) of the server, also known
        ///     as the local or private IP.
        /// </summary>
        public IPEndPoint InternalAddress { get; }

        /// <summary>
        ///     The public IP address (in front of the NAT) of the server, also
        ///     known as the public ip address (what the client will connect to)
        /// </summary>
        public IPEndPoint ExternalAddress { get; }

        /// <summary>
        ///     Time of the last ping of the server, used to determine if the server
        ///     is still running.
        /// </summary>
        public DateTime LastPing { get; }

        /// <summary>
        ///     The display name of the server, shown in the public server list.
        /// </summary>
        public string ServerName { get; }

        /// <summary>
        ///     The maximum amount of players that can connect to this server.
        /// </summary>
        public int MaxPlayers { get; }

        /// <summary>
        ///     The current amount of connected players.
        /// </summary>
        public int CurrentPlayers { get; }

        /// <summary>
        ///     Whether a password is required to join this server.
        /// </summary>
        public bool HasPassword { get; }

        /// <summary>
        ///     Whether this server should be listed in the public server list.
        /// </summary>
        public bool ListPublicly { get; }

        public Server(IPEndPoint internalAddress, IPEndPoint externalAddress, string token,
            string serverName = null, int maxPlayers = 0, int currentPlayers = 0,
            bool hasPassword = false, bool listPublicly = false)
        {
            LastPing = DateTime.Now;
            Token = token;
            InternalAddress = internalAddress;
            ExternalAddress = externalAddress;
            ServerName = serverName;
            MaxPlayers = maxPlayers;
            CurrentPlayers = currentPlayers;
            HasPassword = hasPassword;
            ListPublicly = listPublicly;
        }
    }
}
