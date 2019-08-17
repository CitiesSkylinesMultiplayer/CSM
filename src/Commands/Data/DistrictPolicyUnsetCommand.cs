using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public class DistrictPolicyUnsetCommand : CommandBase
    {
        [ProtoMember(1)]
        public DistrictPolicies.Policies Policy { get; set; }

        [ProtoMember(2)]
        public byte DistrictId { get; set; }
    }
}
