using ProtoBuf;

namespace CSM.Commands.Data.Internal
{
    [ProtoContract]
    class WorldTransferCommand : CommandBase
    {
        [ProtoMember(1)]
        public byte[] World { get; set; }
    }
}
