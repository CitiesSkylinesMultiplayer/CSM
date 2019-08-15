using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public class ChangeCityNameCommand : CommandBase
    {
        [ProtoMember(1)]
        public string Name { get; set; }
    }
}
