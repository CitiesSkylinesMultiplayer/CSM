using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public class ZoneUpdateCommand : CommandBase
    {
        [ProtoMember(1)]
        public ushort ZoneId { get; set; }

        [ProtoMember(2)]
        public ulong Zone1 { get; set; }

        [ProtoMember(3)]
        public ulong Zone2 { get; set; }
    }
}
