using ProtoBuf;

namespace CSM.Commands.Data.Net
{
    /// <summary>
    ///     Called when the name seed of a segment is updated.
    /// </summary>
    /// Sent by:
    /// - NetHandler
    [ProtoContract]
    public class SegmentSeedUpdateCommand : CommandBase
    {
        /// <summary>
        ///     The segment id to modify.
        /// </summary>
        [ProtoMember(1)]
        public ushort SegmentId { get; set; }
        
        /// <summary>
        ///     The new name seed.
        /// </summary>
        [ProtoMember(2)]
        public ushort NameSeed { get; set; }
    }
}
