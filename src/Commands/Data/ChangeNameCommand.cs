using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public class ChangeNameCommand : CommandBase
    {
        [ProtoMember(1)]
        public InstanceType Type { get; set; }

        [ProtoMember(2)]
        public int Id { get; set; }

        [ProtoMember(3)]
        public string Name { get; set; }
    }
}
