using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public class UnlockAreaCommand: CommandBase
    {
        [ProtoMember(1)]
        public int X { get; set; }

        [ProtoMember(2)]
        public int Z { get; set; }
    }
}
