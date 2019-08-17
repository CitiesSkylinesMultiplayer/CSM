using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public class DistrictCreateCommand : CommandBase
    {
        [ProtoMember(1)]
        public byte DistrictId { get; set; }

        [ProtoMember(2)]
        public ulong Seed { get; set; }
    }
}
