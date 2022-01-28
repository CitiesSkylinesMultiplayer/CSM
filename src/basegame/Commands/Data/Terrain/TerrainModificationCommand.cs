using CSM.API.Commands;
using ProtoBuf;
using UnityEngine;

namespace CSM.BaseGame.Commands.Data.Terrain
{
    /// <summary>
    ///     Called when the terrain is modified.
    /// </summary>
    /// Sent by:
    /// - TerrainHandler
    [ProtoContract]
    public class TerrainModificationCommand : CommandBase
    {
        /// <summary>
        ///     The BrushData.
        /// </summary>
        [ProtoMember(1)]
        public float[] BrushData;

        /// <summary>
        ///     The size of the brush.
        /// </summary>
        [ProtoMember(2)]
        public float BrushSize;

        /// <summary>
        ///     The strength of the brush.
        /// </summary>
        [ProtoMember(3)]
        public float Strength;

        /// <summary>
        ///     The current mouse position.
        /// </summary>
        [ProtoMember(4)]
        public Vector3 MousePosition;

        /// <summary>
        ///     The start position of the modification.
        /// </summary>
        [ProtoMember(5)]
        public Vector3 StartPosition;

        /// <summary>
        ///     The end position of the modification.
        /// </summary>
        [ProtoMember(6)]
        public Vector3 EndPosition;

        /// <summary>
        ///     The terrain tool mode.
        /// </summary>
        [ProtoMember(7)]
        public TerrainTool.Mode Mode;

        /// <summary>
        ///     If the right mouse button is pressed.
        /// </summary>
        [ProtoMember(8)]
        public bool MouseRightDown;
    }
}
