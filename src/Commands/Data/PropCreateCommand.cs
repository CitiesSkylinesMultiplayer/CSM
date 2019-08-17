using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class PropCreateCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort PropId;

        [ProtoMember(2)]
        public Vector3 Position;

        [ProtoMember(3)]
        public float Angle;

        [ProtoMember(4)]
        public bool Single;

        [ProtoMember(5)]
        public ushort InfoIndex;
    }
}
