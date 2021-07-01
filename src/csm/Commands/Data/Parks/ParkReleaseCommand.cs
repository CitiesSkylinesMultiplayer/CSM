using CSM.API.Commands;
using ProtoBuf;

namespace CSM.Commands.Data.Parks
{
    /// <summary>
    ///     Called when a park is released.
    /// </summary>
    /// Sent by:
    /// - DistrictHandler
    [ProtoContract]
    public class ParkReleaseCommand : CommandBase
    {
        /// <summary>
        ///     The id of the park to release.
        /// </summary>
        [ProtoMember(1)]
        public byte ParkId { get; set; }
    }
}
