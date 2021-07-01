using CSM.API.Commands;
using ProtoBuf;
using UnityEngine;

namespace CSM.Commands.Data.Districts
{
    /// <summary>
    ///     When the area of a district was modified.
    /// </summary>
    /// Sent by:
    /// - DistrictHandler
    [ProtoContract]
    public class DistrictAreaModifyCommand : CommandBase
    {
        /// <summary>
        ///     The mode of the tool (e.g. districts/parks)
        /// </summary>
        [ProtoMember(1)]
        public DistrictTool.Layer Layer { get; set; }

        /// <summary>
        ///     The id of the district.
        /// </summary>
        [ProtoMember(2)]
        public byte District { get; set; }

        /// <summary>
        ///     The current brush radius.
        /// </summary>
        [ProtoMember(3)]
        public float BrushRadius { get; set; }

        /// <summary>
        ///     The start position of the brush action.
        /// </summary>
        [ProtoMember(4)]
        public Vector3 StartPosition { get; set; }

        /// <summary>
        ///     The end position of the brush action.
        /// </summary>
        [ProtoMember(5)]
        public Vector3 EndPosition { get; set; }
    }
}
