using ProtoBuf;

namespace CSM.Commands.Data.Internal
{
    /// <summary>
    ///     This packet is sent when a transaction is completed (at the end of the current tick).
    /// </summary>
    /// Sent by:
    /// - TransactionHandler
    [ProtoContract]
    [FixedCommand(90010)]
    public class FinishTransactionCommand : CommandBase
    {
    }
}
