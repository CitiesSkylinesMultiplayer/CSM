using CSM.API.Commands;
using ProtoBuf;

namespace CSM.Commands.Data.Districts
{
    /// <summary>
    ///     Called when a city policy was removed.
    /// </summary>
    /// Sent by:
    /// - DistrictHandler
    [ProtoContract]
    public class DistrictCityPolicyUnsetCommand : CommandBase
    {
        /// <summary>
        ///     The city policy that was unset.
        /// </summary>
        [ProtoMember(1)]
        public DistrictPolicies.Policies Policy { get; set; }
    }
}
