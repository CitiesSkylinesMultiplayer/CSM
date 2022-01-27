using ProtoBuf;

namespace CSM.Commands.Data.TransportLines
{
    /// <summary>
    ///     Called when the vehicle type of a transport line was changed.
    /// </summary>
    /// Sent by:
    /// - TransportHandler
    [ProtoContract]
    public class TransportLineChangeVehicleCommand : CommandBase
    {
        /// <summary>
        ///     The id of the line that was modified.
        /// </summary>
        [ProtoMember(1)]
        public ushort LineId;

        /// <summary>
        ///     The prefab info index of the new vehicle.
        /// </summary>
        [ProtoMember(2)]
        public uint? Vehicle;
    }
}
