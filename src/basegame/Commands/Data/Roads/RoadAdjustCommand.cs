using System.Collections.Generic;
using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Roads
{
    /// <summary>
    ///     Sent when the player adjusts the extents of a road through the route menu.
    /// </summary>
    /// Sent by: RoadHandler
    [ProtoContract]
    public class RoadAdjustCommand : CommandBase
    {
        /// <summary>
        ///     The segments previously belonging to the edited road.
        /// </summary>
        [ProtoMember(1)]
        public HashSet<ushort> OriginalSegments { get; set; }

        /// <summary>
        ///     The segments now belonging to the edited road.
        /// </summary>
        [ProtoMember(2)]
        public HashSet<ushort> IncludedSegments { get; set; }

        /// <summary>
        ///     The selected road segment instance.
        /// </summary>
        [ProtoMember(3)]
        public InstanceID LastInstance { get; set; }
    }
}
