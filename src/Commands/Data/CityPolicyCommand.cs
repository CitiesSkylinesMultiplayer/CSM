using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class CityPolicyCommand : CommandBase
    {
        [ProtoMember(1)]
        public DistrictPolicies.Policies Policy { get; set; }

    }
}
