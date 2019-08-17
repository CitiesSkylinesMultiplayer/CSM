using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public class DistrictCityPolicyUnsetCommand : CommandBase
    {
        [ProtoMember(1)]
        public DistrictPolicies.Policies Policy { get; set; }
    }
}
