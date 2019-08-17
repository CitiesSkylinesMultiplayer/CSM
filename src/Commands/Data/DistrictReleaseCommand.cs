using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public class DistrictReleaseCommand : CommandBase
    {
        [ProtoMember(1)]
        public byte DistrictId { get; set; }
    }
}
