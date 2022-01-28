using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Buildings
{
    /// <summary>
    ///     Called when the historical state of a building was changed.
    /// </summary>
    /// Sent by:
    /// - BuildingHandler
    [ProtoContract]
    public class BuildingSetHistoricalCommand : CommandBase
    {
        /// <summary>
        ///     The id of the modified building.
        /// </summary>
        [ProtoMember(1)]
        public ushort Building { get; set; }

        /// <summary>
        ///     The new historical state.
        /// </summary>
        [ProtoMember(2)]
        public bool Historical { get; set; }
    }
}
