using ProtoBuf;

namespace CSM.Commands
{
    /// <summary>
    ///     This sends the current cash amount and sync the income and expenses shown on the UI
    /// </summary>
    [ProtoContract]
    public class MoneyCommand : CommandBase
    {
        [ProtoMember(1)]
        public long MoneyAmount { get; set; }

        [ProtoMember(2)]
        public long[] TotalIncome { get; set; }

        [ProtoMember(3)]
        public long[] TotalExpenses { get; set; }
    }
}