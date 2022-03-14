using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Buildings
{
    /// <summary>
    ///     Called when the production rate of a building is changed.
    ///     This includes turning the building on or off
    /// </summary>
    /// Sent by:
    /// - BuildingHandler
    [ProtoContract]
    public class BuildingChangeProductionRateCommand : CommandBase
    {
        /// <summary>
        ///     The id of the building that was changed.
        /// </summary>
        [ProtoMember(1)]
        public ushort Building { get; set; }

        /// <summary>
        ///     The new production rate of the building.
        /// </summary>
        [ProtoMember(2)]
        public byte Rate { get; set; }
    }
}
