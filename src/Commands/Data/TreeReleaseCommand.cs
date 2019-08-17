using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public class TreeReleaseCommand : CommandBase
    {
        [ProtoMember(1)]
        public uint TreeId { get; set; }
    }
}
