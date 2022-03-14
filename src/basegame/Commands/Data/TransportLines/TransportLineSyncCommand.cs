using CSM.API.Commands;
using ProtoBuf;
using UnityEngine;

namespace CSM.BaseGame.Commands.Data.TransportLines
{
    /// <summary>
    ///     Called when some tool information was changed during tick processing.
    /// </summary>
    /// Sent by:
    /// - TransportHandler
    [ProtoContract]
    public class TransportLineSyncCommand : CommandBase
    {
        /// <summary>
        ///     The current hit position of the mouse cursor.
        /// </summary>
        [ProtoMember(1)]
        public Vector3 HitPosition { get; set; }

        /// <summary>
        ///     If the stops of the current line have a fixed platform.
        /// </summary>
        [ProtoMember(2)]
        public bool FixedPlatform { get; set; }

        /// <summary>
        ///     The index of the stop that was hovered.
        /// </summary>
        [ProtoMember(3)]
        public int HoverStopIndex { get; set; }

        /// <summary>
        ///     The index of the segment that was hovered.
        /// </summary>
        [ProtoMember(4)]
        public int HoverSegmentIndex { get; set; }

        /// <summary>
        ///     The current mode of the tool.
        /// </summary>
        [ProtoMember(5)]
        public int Mode { get; set; }

        /// <summary>
        ///     The list of current errors.
        /// </summary>
        [ProtoMember(6)]
        public ToolBase.ToolErrors Errors { get; set; }

        /// <summary>
        ///     If TransportManager::UpdateLinesNow was called during tick processing.
        /// </summary>
        [ProtoMember(7)]
        public bool UpdateLines { get; set; }

        /// <summary>
        ///     If TransportLine::UpdatePaths was called on the temp line during tick processing.
        /// </summary>
        [ProtoMember(8)]
        public bool UpdatePaths { get; set; }
    }
}
