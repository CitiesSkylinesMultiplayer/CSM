using CSM.API.Commands;
using ProtoBuf;

namespace CSM.Commands.Data.Economy
{
    /// <summary>
    ///     Sent when a loan has been taken.
    /// </summary>
    /// Sent by:
    /// - EconomyHandler
    [ProtoContract]
    public class EconomyTakeLoanCommand : CommandBase
    {
        /// <summary>
        ///     The index of the loan.
        /// </summary>
        [ProtoMember(1)]
        public int Index { get; set; }

        /// <summary>
        ///     The loan amount.
        /// </summary>
        [ProtoMember(2)]
        public int Amount { get; set; }

        /// <summary>
        ///     The interest of the loan.
        /// </summary>
        [ProtoMember(3)]
        public int Interest { get; set; }

        /// <summary>
        ///     The length of the loan.
        /// </summary>
        [ProtoMember(4)]
        public int Length { get; set; }
    }
}
