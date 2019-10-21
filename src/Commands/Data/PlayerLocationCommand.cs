using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    /// <summary>
    ///     Send chat messages to other players in game
    /// </summary>
    [ProtoContract]
    public class PlayerLocationCommand : CommandBase
    {
        [ProtoMember(1)]
        public int playerId { get; set; }

        [ProtoMember(2)]
        public string playerName { get; set; }

        [ProtoMember(3)]
        public Vector3 playerCameraPosition { get; set; }

        [ProtoMember(4)]
        public Quaternion playerCameraRotation { get; set; }

        [ProtoMember(5)]
        public float playerCameraHeight { get; set; }

        [ProtoMember(6)]
        public Color playerColor { get; set; }
        
    }
}
