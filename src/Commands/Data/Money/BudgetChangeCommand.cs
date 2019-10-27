using ProtoBuf;

namespace CSM.Commands.Data.Money
{
    /// <summary>
    ///     Called when the budget settings have been changed.
    /// </summary>
    /// Sent by:
    /// - EconomyExtension
    [ProtoContract]
    public class BudgetChangeCommand : CommandBase
    {
        /// <summary>
        ///     The list of daytime budgets.
        /// </summary>
        [ProtoMember(1)]
        public int[] ServiceBudgetDay { get; set; }

        /// <summary>
        ///     The list of nighttime budgets.
        /// </summary>
        [ProtoMember(2)]
        public int[] ServiceBudgetNight { get; set; }
    }
}
