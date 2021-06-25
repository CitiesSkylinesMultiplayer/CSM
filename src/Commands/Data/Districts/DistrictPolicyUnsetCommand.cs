using CSM.API.Commands;
using ProtoBuf;

namespace CSM.Commands.Data.Districts
{
    /// <summary>
    ///     Called when a district policy is unset.
    /// </summary>
    /// Sent by:
    /// - DistrictHandler
    [ProtoContract]
    public class DistrictPolicyUnsetCommand : CommandBase
    {
        /// <summary>
        ///     The district policy that was unset.
        /// </summary>
        [ProtoMember(1)]
        public DistrictPolicies.Policies Policy { get; set; }

        /// <summary>
        ///     The modified district.
        /// </summary>
        [ProtoMember(2)]
        public byte DistrictId { get; set; }
    }
}
