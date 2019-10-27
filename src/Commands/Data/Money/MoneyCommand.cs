using ProtoBuf;

namespace CSM.Commands.Data.Money
{
    /// <summary>
    ///     This sends the current cash amount and syncs the income and expenses shown on the UI.
    /// </summary>
    /// Sent by:
    /// - EconomyExtension
    [ProtoContract]
    public class MoneyCommand : CommandBase
    {
        /// <summary>
        ///     The current amount of money.
        /// </summary>
        [ProtoMember(1)]
        public long MoneyAmount { get; set; }

        /// <summary>
        ///     The list of incomes.
        /// </summary>
        [ProtoMember(2)]
        public long[] TotalIncome { get; set; }

        /// <summary>
        ///     The list of expenses.
        /// </summary>
        [ProtoMember(3)]
        public long[] TotalExpenses { get; set; }
    }
}
