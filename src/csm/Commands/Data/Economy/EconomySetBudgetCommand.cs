using CSM.API.Commands;
using ProtoBuf;

namespace CSM.Commands.Data.Economy
{
    /// <summary>
    ///     Called when the budget of a service was changed.
    /// </summary>
    /// Sent by:
    /// - EconomyHandler
    [ProtoContract]
    public class EconomySetBudgetCommand : CommandBase
    {
        /// <summary>
        ///     The modified service.
        /// </summary>
        [ProtoMember(1)]
        public ItemClass.Service Service { get; set; }

        /// <summary>
        ///     The modified subservice.
        /// </summary>
        [ProtoMember(2)]
        public ItemClass.SubService SubService { get; set; }

        /// <summary>
        ///     The new budget.
        /// </summary>
        [ProtoMember(3)]
        public int Budget { get; set; }

        /// <summary>
        ///     If the budget for the day or night was modified.
        /// </summary>
        [ProtoMember(4)]
        public bool Night { get; set; }
    }
}
