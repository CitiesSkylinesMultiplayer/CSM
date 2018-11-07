using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public class NodeIdCommand : CommandBase
    {
        [ProtoMember(1)]
        public uint NodeIdSender { get; set; }

        [ProtoMember(2)]
        public uint NodeIdReciever { get; set; }
    }
}