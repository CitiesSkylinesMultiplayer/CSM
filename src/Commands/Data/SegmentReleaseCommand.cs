using ProtoBuf;

namespace CSM.Commands
{
    public class SegmentReleaseCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort SegmentId { get; set; }
    }
}