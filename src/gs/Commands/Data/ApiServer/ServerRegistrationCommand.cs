using ProtoBuf;

namespace CSM.GS.Commands.Data.ApiServer
{
    /// <summary>
    ///     Registers the game server on the API server to enable NAT hole punching.
    /// </summary>
    /// Sent by:
    /// - Server
    [ProtoContract]
    public class ServerRegistrationCommand : ApiCommandBase
    {
        /// <summary>
        ///     The server token to register.
        /// </summary>
        [ProtoMember(1)]
        public string Token { get; set; }

        /// <summary>
        ///     The server's IP address in the local network.
        /// </summary>
        [ProtoMember(2)]
        public string LocalIp { get; set; }

        /// <summary>
        ///     The configured local port.
        /// </summary>
        [ProtoMember(3)]
        public int LocalPort { get; set; }

        /// <summary>
        ///     The display name of the server, shown in the public server list.
        /// </summary>
        [ProtoMember(4)]
        public string ServerName { get; set; }

        /// <summary>
        ///     The maximum amount of players that can connect to this server.
        /// </summary>
        [ProtoMember(5)]
        public int MaxPlayers { get; set; }

        /// <summary>
        ///     The current amount of connected players.
        /// </summary>
        [ProtoMember(6)]
        public int CurrentPlayers { get; set; }

        /// <summary>
        ///     Whether a password is required to join this server.
        /// </summary>
        [ProtoMember(7)]
        public bool HasPassword { get; set; }

        /// <summary>
        ///     Whether this server should be listed in the public server list.
        /// </summary>
        [ProtoMember(8)]
        public bool ListPublicly { get; set; }
    }
}
