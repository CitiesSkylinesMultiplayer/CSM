using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class BuildingCreatedCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort BuildingID { get; set; }

        [ProtoMember(2)]
        public Vector3 Position { get; set; }

        [ProtoMember(3)]
        public ushort Infoindex { get; set; }

        [ProtoMember(4)]
        public float Angel { get; set; }

        [ProtoMember(5)]
        public int Length { get; set; }
    }
}