using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public class NodeReleaseCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort NodeId { get; set; }
    }
}
