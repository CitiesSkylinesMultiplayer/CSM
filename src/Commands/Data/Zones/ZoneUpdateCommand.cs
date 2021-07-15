using ProtoBuf;

namespace CSM.Commands.Data.Zones
{
    /// <summary>
    ///     Called when the zoning is changed.
    /// </summary>
    /// Sent by:
    /// - ZoneHandler
    [ProtoContract]
    public class ZoneUpdateCommand : CommandBase
    {
        /// <summary>
        ///     The zone id to modify.
        /// </summary>
        [ProtoMember(1)]
        public ushort ZoneId { get; set; }

        /// <summary>
        ///     The data for the first half of the zone.
        /// </summary>
        [ProtoMember(2)]
        public ulong Zone1 { get; set; }

        /// <summary>
        ///     The data for the second half of the zone.
        /// </summary>
        [ProtoMember(3)]
        public ulong Zone2 { get; set; }
    }
}
