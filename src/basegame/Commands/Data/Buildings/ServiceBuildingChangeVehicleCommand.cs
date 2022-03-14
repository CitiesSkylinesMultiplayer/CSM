using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Buildings
{
    /// <summary>
    ///     Called when the vehicle type of a service building was changed.
    /// </summary>
    /// Sent by:
    /// - BuildingHandler
    [ProtoContract]
    public class ServiceBuildingChangeVehicleCommand : CommandBase
    {
        /// <summary>
        ///     The id of the service building that was modified.
        /// </summary>
        [ProtoMember(1)]
        public ushort BuildingId;

        /// <summary>
        ///     The prefab info index of the new vehicle.
        /// </summary>
        [ProtoMember(2)]
        public uint? Vehicle;
    }
}
