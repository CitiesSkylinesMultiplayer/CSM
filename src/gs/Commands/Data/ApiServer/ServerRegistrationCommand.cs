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
    }
}
