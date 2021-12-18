using CSM.API.Commands;
using ProtoBuf;
using UnityEngine;

namespace CSM.Commands.Data.Buildings
{
    /// <summary>
    ///     This command is sent when a building is created (BuildingTool).
    /// </summary>
    /// Sent by:
    /// - BuildingManager
    [ProtoContract]
    public class BuildingToolCreateCommand : CommandBase
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
        ///     The info index of the building's prefab.
        /// </summary>
        [ProtoMember(3)]
        public ushort Prefab { get; set; }

        /// <summary>
        ///     If the building was relocated.
        /// </summary>
        [ProtoMember(4)]
        public int Relocate { get; set; }

        /// <summary>
        ///     The colliding segments array of the ToolController.
        /// </summary>
        [ProtoMember(5)]
        public ulong[] CollidingSegments { get; set; }

        /// <summary>
        ///     The colliding buildings array of the ToolController.
        /// </summary>
        [ProtoMember(6)]
        public ulong[] CollidingBuildings { get; set; }

        /// <summary>
        ///     The position to create the building on.
        /// </summary>
        [ProtoMember(7)]
        public Vector3 MousePosition { get; set; }

        /// <summary>
        ///     The angle to create the building on.
        /// </summary>
        [ProtoMember(8)]
        public float MouseAngle { get; set; }

        /// <summary>
        ///     The elevation to create the building on.
        /// </summary>
        [ProtoMember(9)]
        public int Elevation { get; set; }
    }
}
