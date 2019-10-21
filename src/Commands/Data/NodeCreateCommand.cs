using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class NodeCreateCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort[] Array16Ids { get; set; }
        
        [ProtoMember(2)]
        public uint[] Array32Ids { get; set; }

        [ProtoMember(3)]
        public ushort Prefab { get; set; }

        [ProtoMember(4)]
        public NetTool.ControlPoint StartPoint { get; set; }
        
        [ProtoMember(5)]
        public NetTool.ControlPoint MiddlePoint { get; set; }
        
        [ProtoMember(6)]
        public NetTool.ControlPoint EndPoint { get; set; }

        [ProtoMember(7)]
        public int MaxSegments { get; set; }

        [ProtoMember(8)]
        public bool TestEnds { get; set; }
        
        [ProtoMember(9)]
        public bool AutoFix { get; set; }
        
        [ProtoMember(10)]
        public bool Invert { get; set; }
        
        [ProtoMember(11)]
        public bool SwitchDir { get; set; }
        
        [ProtoMember(12)]
        public ushort RelocateBuildingId { get; set; }
    }
}
