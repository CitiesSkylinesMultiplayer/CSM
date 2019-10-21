using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public class DistrictPolicyCommand : CommandBase
    {
        [ProtoMember(1)]
        public DistrictPolicies.Policies Policy { get; set; }

        [ProtoMember(2)]
        public byte DistrictId { get; set; }
    }
}
