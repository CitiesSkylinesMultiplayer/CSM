using ProtoBuf;
using UnityEngine;
using static Building;

namespace CSM.Commands
{
    [ProtoContract]
    public class BuildingCreateExtendedCommand : CommandBase //can optional args be added to the normal one?
    {
        [ProtoMember(1)]
        public ushort BuildingID { get; set; }

        [ProtoMember(2)]
        public Vector3 Position { get; set; }

        [ProtoMember(3)]
        public ushort Infoindex { get; set; }

        [ProtoMember(4)]
        public float Angle { get; set; }

        [ProtoMember(5)]
        public int Length { get; set; }

        [ProtoMember(6)]
        public Flags Flags { get; set; }
    }
}