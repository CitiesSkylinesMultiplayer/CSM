using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class BuildingToolCreateCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort[] Array16Ids { get; set; }

        [ProtoMember(2)]
        public uint[] Array32Ids { get; set; }

        [ProtoMember(3)]
        public ushort Prefab { get; set; }

        [ProtoMember(4)]
        public int Relocate { get; set; }

        [ProtoMember(5)]
        public ulong[] CollidingSegments { get; set; }

        [ProtoMember(6)]
        public ulong[] CollidingBuildings { get; set; }

        [ProtoMember(7)]
        public Vector3 MousePosition { get; set; }

        [ProtoMember(8)]
        public float MouseAngle { get; set; }

        [ProtoMember(9)]
        public int Elevation { get; set; }
    }
}
