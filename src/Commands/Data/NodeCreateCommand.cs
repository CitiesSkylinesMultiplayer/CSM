using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class NodeCreateCommand : CommandBase
    {
        [ProtoMember(1)]
        public Vector3 Position { get; set; }

        [ProtoMember(2)]
        public ushort InfoIndex { get; set; }

        [ProtoMember(3)]
        public ushort NodeId { get; set; }
    }
}
