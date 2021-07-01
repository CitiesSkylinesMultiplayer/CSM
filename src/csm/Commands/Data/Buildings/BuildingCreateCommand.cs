using CSM.API.Commands;
using ProtoBuf;
using UnityEngine;

namespace CSM.Commands.Data.Buildings
{
    /// <summary>
    ///     This command is sent when a building is created (BuildingManager).
    /// </summary>
    /// Sent by:
    /// - BuildingHandler
    [ProtoContract]
    public class BuildingCreateCommand : CommandBase
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
        ///     The position to create the building.
        /// </summary>
        [ProtoMember(3)]
        public Vector3 Position { get; set; }

        /// <summary>
        ///     The info index of the building's prefab.
        /// </summary>
        [ProtoMember(4)]
        public ushort InfoIndex { get; set; }

        /// <summary>
        ///     The angle of the building.
        /// </summary>
        [ProtoMember(5)]
        public float Angle { get; set; }

        /// <summary>
        ///     The length of the building.
        /// </summary>
        [ProtoMember(6)]
        public int Length { get; set; }
    }
}
