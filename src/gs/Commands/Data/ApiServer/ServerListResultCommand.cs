using ProtoBuf;

namespace CSM.GS.Commands.Data.ApiServer
{
    /// <summary>
    ///     The response to a ServerListRequestCommand, containing the current
    ///     list of publicly-listed servers.
    /// </summary>
    /// Sent by:
    /// - API server
    [ProtoContract]
    public class ServerListResultCommand : ApiCommandBase
    {
        [ProtoMember(1)]
        public PublicServerEntry[] Servers { get; set; }
    }
}
