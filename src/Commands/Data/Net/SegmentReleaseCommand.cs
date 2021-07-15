using ProtoBuf;

namespace CSM.Commands.Data.Net
{
    /// <summary>
    ///     Called when a segment is released.
    /// </summary>
    /// Sent by:
    /// - NetHandler
    [ProtoContract]
    public class SegmentReleaseCommand : CommandBase
    {
        /// <summary>
        ///     The segment id that is being released.
        /// </summary>
        [ProtoMember(1)]
        public ushort SegmentId { get; set; }

        /// <summary>
        ///     If the adjacent nodes should be kept.
        /// </summary>
        [ProtoMember(2)]
        public bool KeepNodes { get; set; }
    }
}
