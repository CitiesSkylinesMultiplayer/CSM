using ProtoBuf;

namespace CSM.Commands.Data.Economy
{
    /// <summary>
    ///     Called when the tax rate was modified.
    /// </summary>
    /// Sent by:
    /// - EconomyHandler
    [ProtoContract]
    public class EconomySetTaxRateCommand : CommandBase
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
        ///     The modified level.
        /// </summary>
        [ProtoMember(3)]
        public ItemClass.Level Level { get; set; }

        /// <summary>
        ///     The new tax rate.
        /// </summary>
        [ProtoMember(4)]
        public int Rate { get; set; }
    }
}
