using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    class PropMoveCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort PropID { get; set; }

        [ProtoMember(2)]
        public Vector3 Position { get; set; }
    }
}
