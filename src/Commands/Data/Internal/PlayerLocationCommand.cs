using ProtoBuf;
using UnityEngine;

namespace CSM.Commands.Data.Internal
{
    /// <summary>
    ///     This commands transfers the player camera positions to display where other players are looking at.
    /// </summary>
    /// Sent by:
    /// - ICameraHandler
    [ProtoContract]
    public class PlayerLocationCommand : CommandBase
    {
        /// <summary>
        ///     The name of the player.
        /// </summary>
        [ProtoMember(1)]
        public string PlayerName { get; set; }

        /// <summary>
        ///     The current camera position.
        /// </summary>
        [ProtoMember(2)]
        public Vector3 PlayerCameraPosition { get; set; }

        /// <summary>
        ///     The current camera rotation.
        /// </summary>
        [ProtoMember(3)]
        public Quaternion PlayerCameraRotation { get; set; }

        /// <summary>
        ///     The current camera height.
        /// </summary>
        [ProtoMember(4)]
        public float PlayerCameraHeight { get; set; }

        /// <summary>
        ///     The configured player color.
        /// </summary>
        [ProtoMember(5)]
        public Color PlayerColor { get; set; }
    }
}
