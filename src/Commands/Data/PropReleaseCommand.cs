using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public class PropReleaseCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort PropId { get; set; }
    }
}
