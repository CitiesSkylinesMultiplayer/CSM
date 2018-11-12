using ProtoBuf;
namespace CSM.Commands
{
    [ProtoContract]
    public class NodeUpdateCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort Segment0 { get; set; }

        [ProtoMember(2)]
        public ushort Segment1 { get; set; }

        [ProtoMember(3)]
        public ushort Segment2 { get; set; }

        [ProtoMember(4)]
        public ushort Segment3 { get; set; }

        [ProtoMember(5)]
        public ushort Segment4 { get; set; }

        [ProtoMember(6)]
        public ushort Segment5 { get; set; }

        [ProtoMember(7)]
        public ushort Segment6 { get; set; }

        [ProtoMember(8)]
        public ushort Segment7 { get; set; }

        [ProtoMember(9)]
        public ushort NodeID { get; set; }
    }
}
