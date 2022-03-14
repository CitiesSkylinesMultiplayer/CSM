using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Buildings
{
    /// <summary>
    ///     Called when a building is upgraded.
    /// </summary>
    /// Sent by:
    /// - BuildingHandler
    [ProtoContract]
    public class BuildingUpgradeCommand : CommandBase
    {
        /// <summary>
        ///     The list of generated Array16 ids collected by the ArrayHandler.
        /// </summary>
        [ProtoMember(1)]
        public ushort[] Array16Ids { get; set; }

        /// <summary>
        ///     The list of generated Array32 ids collected by the ArrayHandler.
        /// </summary>
        [ProtoMember(2)]
        public uint[] Array32Ids { get; set; }

        /// <summary>
        ///     The id of the building that was upgraded.
        /// </summary>
        [ProtoMember(3)]
        public ushort Building { get; set; }
    }
}
