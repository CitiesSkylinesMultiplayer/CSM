using ProtoBuf;

namespace CSM.Commands.Data.Net
{
    /// <summary>
    ///     Called when a net node gets created through the NetTool.
    /// </summary>
    /// Sent by:
    /// - NetHandler
    [ProtoContract]
    public class NodeCreateCommand : CommandBase
    {
        /// <summary>
        ///     The list of generated Array16 ids collected by the ArrayHandler.
        /// </summary>
        [ProtoMember(1)]
        public ushort[] Array16Ids { get; set; }
        
        /// <summary>
        ///     The list of generated Array32 ids collected by the ArrayHandler.
        /// </summary>
        [ProtoMember(2)]
        public uint[] Array32Ids { get; set; }

        /// <summary>
        ///     The info index of the prefab to create.
        /// </summary>
        [ProtoMember(3)]
        public ushort Prefab { get; set; }
        
        /// <summary>
        ///     The start point of the created net part.
        /// </summary>
        [ProtoMember(4)]
        public NetTool.ControlPoint StartPoint { get; set; }
        
        /// <summary>
        ///     The middle point of the created net part.
        /// </summary>
        [ProtoMember(5)]
        public NetTool.ControlPoint MiddlePoint { get; set; }
        
        /// <summary>
        ///     The end point of the created net part.
        /// </summary>
        [ProtoMember(6)]
        public NetTool.ControlPoint EndPoint { get; set; }

        /// <summary>
        ///     The maximum number of segments to create.
        /// </summary>
        [ProtoMember(7)]
        public int MaxSegments { get; set; }

        /// <summary>
        ///     TestEnds
        /// </summary>
        [ProtoMember(8)]
        public bool TestEnds { get; set; }
        
        /// <summary>
        ///     AutoFix
        /// </summary>
        [ProtoMember(9)]
        public bool AutoFix { get; set; }
        
        /// <summary>
        ///     If the invert mode is enabled.
        /// </summary>
        [ProtoMember(10)]
        public bool Invert { get; set; }
        
        /// <summary>
        ///     If the direction should be changed.
        /// </summary>
        [ProtoMember(11)]
        public bool SwitchDir { get; set; }
        
        /// <summary>
        ///     The building id that is being relocated.
        /// </summary>
        [ProtoMember(12)]
        public ushort RelocateBuildingId { get; set; }
    }
}
