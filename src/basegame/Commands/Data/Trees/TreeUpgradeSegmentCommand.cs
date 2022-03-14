using CSM.API.Commands;
using ProtoBuf;
using UnityEngine;

namespace CSM.BaseGame.Commands.Data.Trees
{
    /// <summary>
    ///     Called when the tree type on a road segment is changed.
    /// </summary>
    /// Sent by:
    /// - TreeHandler
    [ProtoContract]
    public class TreeUpgradeSegmentCommand : CommandBase
    {
        /// <summary>
        ///     The id of the modified segment.
        /// </summary>
        [ProtoMember(1)]
        public ushort UpgradeSegment { get; set; }

        /// <summary>
        ///     The prefab identifier.
        /// </summary>
        [ProtoMember(2)]
        public ushort Prefab { get; set; }

        /// <summary>
        ///     The mouse position where an effect should be played.
        /// </summary>
        [ProtoMember(3)]
        public Vector3 MousePosition { get; set; }
    }
}
