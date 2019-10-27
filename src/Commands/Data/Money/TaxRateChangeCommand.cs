using ProtoBuf;

namespace CSM.Commands.Data.Money
{
    /// <summary>
    ///     Called when a tax rate was changed.
    /// </summary>
    /// Sent by:
    /// - EconomyExtension
    [ProtoContract]
    public class TaxRateChangeCommand : CommandBase
    {
        /// <summary>
        ///     The list of tax rates.
        /// </summary>
        [ProtoMember(1)]
        public int[] TaxRate { get; set; }
    }
}
