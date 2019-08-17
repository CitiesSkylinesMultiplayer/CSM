using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public class NodeUpdateCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort[] Segments { get; set; }

        [ProtoMember(2)]
        public ushort NodeId { get; set; }

        [ProtoMember(3)]
        public NetNode.Flags Flags { get; set; }
    }
}
