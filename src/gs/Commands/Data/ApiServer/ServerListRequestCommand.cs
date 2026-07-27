using ProtoBuf;

namespace CSM.GS.Commands.Data.ApiServer
{
    /// <summary>
    ///     Requests the current list of publicly-listed servers from the API server.
    /// </summary>
    /// Sent by:
    /// - Mod (browsing client)
    [ProtoContract]
    public class ServerListRequestCommand : ApiCommandBase
    {
    }
}
