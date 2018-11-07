using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public class BuildingIdCommand : CommandBase
    {
        [ProtoMember(1)]
        public uint BuildingIdSender { get; set; }

        [ProtoMember(2)]
        public uint BuildingIdReciever { get; set; }
    }
}