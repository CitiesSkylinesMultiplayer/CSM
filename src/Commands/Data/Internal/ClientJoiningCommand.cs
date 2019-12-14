using ProtoBuf;

namespace CSM.Commands.Data.Internal
{
    [ProtoContract]
    class ClientJoiningCommand : CommandBase
    {
        [ProtoMember(1)]
        public bool JoiningFinished { get; set; }
    }
}
