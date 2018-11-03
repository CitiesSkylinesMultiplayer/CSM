using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class NodeCommand : CommandBase
    {
        [ProtoMember(1)]
        public Vector3 Position { get; set; }

        [ProtoMember(2)]
        public ushort InfoIndex { get; set; }

        [ProtoMember(3)]
        public int NodeID { get; set; }
    }
}