using CSM.API.Commands;
using ProtoBuf;
using UnityEngine;

namespace CSM.BaseGame.Commands.Data.Buildings
{
    /// <summary>
    ///     Called when a building is relocated (BuildingManager).
    /// </summary>
    /// Sent by:
    /// - BuildingHandler
    [ProtoContract]
    public class BuildingRelocateCommand : CommandBase
    {
        /// <summary>
        ///     The new position.
        /// </summary>
        [ProtoMember(1)]
        public Vector3 NewPosition { get; set; }

        /// <summary>
        ///     The id of the modified building
        /// </summary>
        [ProtoMember(2)]
        public ushort BuildingId { get; set; }

        /// <summary>
        ///     The new angle.
        /// </summary>
        [ProtoMember(3)]
        public float Angle { get; set; }
    }
}
