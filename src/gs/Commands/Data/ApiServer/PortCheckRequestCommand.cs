using ProtoBuf;

namespace CSM.GS.Commands.Data.ApiServer
{
    /// <summary>
    ///     Request the global server to check the given port for connectivity.
    /// </summary>
    /// Sent by:
    /// - Server
    [ProtoContract]
    public class PortCheckRequestCommand : ApiCommandBase
    {
        /// <summary>
        ///     The port to check.
        /// </summary>
        [ProtoMember(1)]
        public int Port { get; set; }
    }
}
