using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public class SegmentSeedUpdateCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort SegmentId { get; set; }
        
        [ProtoMember(2)]
        public ushort NameSeed { get; set; }
    }
}
