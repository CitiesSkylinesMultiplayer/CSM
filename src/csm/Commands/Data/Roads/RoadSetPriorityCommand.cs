using CSM.API.Commands;
using ProtoBuf;

namespace CSM.Commands.Data.Roads
{
    /// <summary>
    ///     Called when the priority state of a road is toggled.
    /// </summary>
    /// Sent by:
    /// - RoadHandler
    [ProtoContract]
    public class RoadSetPriorityCommand : CommandBase
    {
        /// <summary>
        ///     The segment that was toggled.
        /// </summary>
        [ProtoMember(1)]
        public ushort SegmentId { get; set; }

        /// <summary>
        ///     If the road should be a priority road.
        /// </summary>
        [ProtoMember(2)]
        public bool Priority { get; set; }
    }
}
