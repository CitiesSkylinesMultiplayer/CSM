using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public class RoadSetPriorityCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort SegmentId { get; set; }

        [ProtoMember(2)]
        public bool Priority { get; set; }
    }
}
