using ProtoBuf;

namespace CSM.Commands.Data.Internal
{
    /// <summary>
    /// Requests a world transfer/sync, only sent by clients
    /// </summary>
    /// Sent by:
    /// - ChatLogPanel
    [ProtoContract]
    class RequestWorldTransferCommand : CommandBase
    {
    }
}
