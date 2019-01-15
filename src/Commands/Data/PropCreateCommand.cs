using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class PropCreateCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort PropID;

        [ProtoMember(2)]
        public Vector3 position;

        [ProtoMember(3)]
        public float angle;

        [ProtoMember(4)]
        public bool single;

        [ProtoMember(5)]
        public ushort infoindex;



    }
}
