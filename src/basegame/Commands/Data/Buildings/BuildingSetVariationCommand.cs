using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Buildings
{
    /// <summary>
    ///     Called when the building variation was changed.
    /// </summary>
    /// Sent by:
    /// - BuildingHandler
    [ProtoContract]
    public class BuildingSetVariationCommand : CommandBase
    {
        /// <summary>
        ///     The id of the modified building.
        /// </summary>
        [ProtoMember(1)]
        public ushort Building { get; set; }

        /// <summary>
        ///     The new variation flags.
        /// </summary>
        [ProtoMember(2)]
        public Building.Flags2 Variation { get; set; }
    }
}
