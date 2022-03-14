using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Economy
{
    /// <summary>
    ///     Sent when a loan has been paid back.
    /// </summary>
    /// Sent by:
    /// - EconomyHandler
    [ProtoContract]
    public class EconomyPayLoanCommand : CommandBase
    {
        /// <summary>
        ///     The index of the loan.
        /// </summary>
        [ProtoMember(1)]
        public int Index { get; set; }

        /// <summary>
        ///     The amount that was paid back.
        /// </summary>
        [ProtoMember(2)]
        public int Paid { get; set; }
    }
}
