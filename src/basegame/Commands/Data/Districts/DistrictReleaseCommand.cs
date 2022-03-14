using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Districts
{
    /// <summary>
    ///     Called when a district is removed.
    /// </summary>
    /// Sent by:
    /// - DistrictHandler
    [ProtoContract]
    public class DistrictReleaseCommand : CommandBase
    {
        /// <summary>
        ///     The id of the district to remove.
        /// </summary>
        [ProtoMember(1)]
        public byte DistrictId { get; set; }
    }
}
