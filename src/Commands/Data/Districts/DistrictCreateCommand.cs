using ProtoBuf;

namespace CSM.Commands.Data.Districts
{
    /// <summary>
    ///     Called when a district was created.
    /// </summary>
    /// Sent by:
    /// - DistrictHandler
    [ProtoContract]
    public class DistrictCreateCommand : CommandBase
    {
        /// <summary>
        ///     The new district id.
        /// </summary>
        [ProtoMember(1)]
        public byte DistrictId { get; set; }

        /// <summary>
        ///     The random seed of the district (for names, etc.).
        /// </summary>
        [ProtoMember(2)]
        public ulong Seed { get; set; }
    }
}
