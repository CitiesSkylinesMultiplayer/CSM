using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class NodeSegmentCreateCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort StartNode { get; set; }

        [ProtoMember(2)]
        public ushort EndNode { get; set; }

        [ProtoMember(3)]
        public Vector3 StartDirection { get; set; }

        [ProtoMember(4)]
        public Vector3 EndDirection { get; set; }

        [ProtoMember(5)]
        public uint ModifiedIndex { get; set; }

        [ProtoMember(6)]
        public ushort InfoIndex { get; set; }
    }
}