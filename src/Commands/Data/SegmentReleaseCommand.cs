using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public class SegmentReleaseCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort SegmentId { get; set; }

        [ProtoMember(2)]
        public bool KeepNodes { get; set; }
    }
}