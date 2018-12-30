using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class TreeCreateCommand : CommandBase
    {
        [ProtoMember(1)]
        public uint TreeID { get; set; }

        [ProtoMember(2)]
        public Vector3 Position { get; set; }

        [ProtoMember(3)]
        public bool Single { get; set; }

        [ProtoMember(4)]
        public ushort InfoIndex { get; set; }
    }
}
