using CSM.API.Commands;
using ProtoBuf;

namespace CSM.Commands.Data.Districts
{
    /// <summary>
    ///     Called when a city policy was added.
    /// </summary>
    /// Sent by:
    /// - District Handler
    [ProtoContract]
    public class DistrictCityPolicyCommand : CommandBase
    {
        /// <summary>
        ///     The policy that was set.
        /// </summary>
        [ProtoMember(1)]
        public DistrictPolicies.Policies Policy { get; set; }
    }
}
