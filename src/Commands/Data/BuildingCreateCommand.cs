using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    /// <summary>
    ///     This command is sent when a building is created.
    /// </summary>
    [ProtoContract]
    public class BuildingCreateCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort BuildingId { get; set; }

        /// <summary>
        ///     The position to create the building
        /// </summary>
        [ProtoMember(2)]
        public Vector3 Position { get; set; }

        [ProtoMember(3)]
        public ushort InfoIndex { get; set; }

        /// <summary>
        ///     The angle of the building (degrees?, radians?)
        /// </summary>
        [ProtoMember(4)]
        public float Angle { get; set; }

        [ProtoMember(5)]
        public int Length { get; set; }
    }
}