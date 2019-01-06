using ProtoBuf;
using UnityEngine;
using static TerrainTool;

namespace CSM.Commands
{
    [ProtoContract]
    public class TerrainModificationCommand : CommandBase
    {
        [ProtoMember(1)]
        public float[] BrushData;

        [ProtoMember(2)]
        public float BrushSize;

        [ProtoMember(3)]
        public float Strength;

        [ProtoMember(4)]
        public Vector3 mousePosition;

        [ProtoMember(5)]
        public Vector3 StartPosition;

        [ProtoMember(6)]
        public Vector3 EndPosition;

        [ProtoMember(7)]
        public Mode mode;

        [ProtoMember(8)]
        public bool MouseRightDown;

        [ProtoMember(9)]
        public bool MouseLeftDown;



    }
}
