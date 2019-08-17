using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class TreeMoveCommand : CommandBase
    {
        [ProtoMember(1)]
        public uint TreeId { get; set; }

        [ProtoMember(2)]
        public Vector3 Position { get; set; }
    }
}
