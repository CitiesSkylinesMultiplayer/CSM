using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Buildings
{
    /// <summary>
    ///     Called when the industry building variation was changed.
    /// </summary>
    /// Sent by:
    /// - BuildingHandler
    [ProtoContract]
    public class BuildingSetIndustrialVariationCommand : CommandBase
    {
        /// <summary>
        ///     The id of the modified building.
        /// </summary>
        [ProtoMember(1)]
        public ushort Building { get; set; }

        /// <summary>
        ///     The new building info.
        /// </summary>
        [ProtoMember(2)]
        public ushort VariationInfoIndex { get; set; }

        /// <summary>
        ///     The selected variation index.
        /// </summary>
        [ProtoMember(3)]
        public int VariationIndex { get; set; }
    }
}
