using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Net
{
    /// <summary>
    ///     Called when the fence type on a road segment is changed.
    /// </summary>
    /// Sent by:
    /// - NetHandler
    [ProtoContract]
    public class FenceUpgradeSegmentCommand : CommandBase
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
        ///     The upgrade mode.
        /// </summary>
        [ProtoMember(3)]
        public int Mode { get; set; }

        /// <summary>
        ///     The upgrade side.
        /// </summary>
        [ProtoMember(4)]
        public bool Side { get; set; }
    }
}
