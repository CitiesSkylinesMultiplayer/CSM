using ProtoBuf;

namespace CSM.Commands.Data
{
    /// <summary>
    /// This packet is send when a transaction is completed.
    /// </summary>
    [ProtoContract]
    public class FinishTransactionCommand : CommandBase
    {
        [ProtoMember(1)]
        public TransactionType Type;
    }
}
