using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    [ProtoContract]
    public class PlayerLocationCommand : CommandBase
    {
        [ProtoMember(1)]
        public int PlayerId { get; set; }

        [ProtoMember(2)]
        public string PlayerName { get; set; }

        [ProtoMember(3)]
        public Vector3 PlayerCameraPosition { get; set; }

        [ProtoMember(4)]
        public Quaternion PlayerCameraRotation { get; set; }

        [ProtoMember(5)]
        public float PlayerCameraHeight { get; set; }

        [ProtoMember(6)]
        public Color PlayerColor { get; set; }
    }
}
