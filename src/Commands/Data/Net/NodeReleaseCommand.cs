using ProtoBuf;

namespace CSM.Commands.Data.Net
{
    /// <summary>
    ///     Called when a node is released.
    /// </summary>
    /// Sent by:
    /// - NetHandler
    [ProtoContract]
    public class NodeReleaseCommand : CommandBase
    {
        /// <summary>
        ///     The node id to release.
        /// </summary>
        [ProtoMember(1)]
        public ushort NodeId { get; set; }
    }
}
