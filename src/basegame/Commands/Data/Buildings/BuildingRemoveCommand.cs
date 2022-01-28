using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Buildings
{
    /// <summary>
    ///     This command is sent when a building is removed (BuildingManager).
    /// </summary>
    /// Sent by:
    /// - BuildingHandler
    [ProtoContract]
    public class BuildingRemoveCommand : CommandBase
    {
        /// <summary>
        ///     The id of the building to be removed
        /// </summary>
        [ProtoMember(1)]
        public ushort BuildingId { get; set; }
    }
}
