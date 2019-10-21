using ProtoBuf;

namespace CSM.Commands
{
    public class ParkReleaseCommand : CommandBase
    {
        [ProtoMember(1)]
        public byte ParkId { get; set; }
    }
}
