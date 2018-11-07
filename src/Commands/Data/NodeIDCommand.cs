using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    internal class NodeIDCommand : CommandBase
    {
        [ProtoMember(1)]
        public uint NodeIDSender { get; set; }

        [ProtoMember(2)]
        public uint NodeIDReciever { get; set; }
    }
}