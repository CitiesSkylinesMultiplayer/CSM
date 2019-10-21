using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class DistrictAreaModifyCommand : CommandBase
    {
        [ProtoMember(1)]
        public DistrictTool.Layer Layer { get; set; }

        [ProtoMember(2)]
        public byte District { get; set; }

        [ProtoMember(3)]
        public float BrushRadius { get; set; }

        [ProtoMember(4)]
        public Vector3 StartPosition { get; set; }

        [ProtoMember(5)]
        public Vector3 EndPosition { get; set; }
    }
}
