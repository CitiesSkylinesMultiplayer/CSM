using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Buildings
{
    /// <summary>
    ///     Called when the rebuild button is clicked on a destroyed building.
    /// </summary>
    /// Sent by:
    /// - BuildingHandler
    [ProtoContract]
    public class BuildingRebuildCommand : CommandBase
    {
        /// <summary>
        ///     The id of the building that was rebuilt.
        /// </summary>
        [ProtoMember(1)]
        public ushort Building { get; set; }

        /// <summary>
        ///     The list of generated Array16 ids collected by the ArrayHandler.
        /// </summary>
        [ProtoMember(2)]
        public ushort[] Array16Ids { get; set; }
    }
}
