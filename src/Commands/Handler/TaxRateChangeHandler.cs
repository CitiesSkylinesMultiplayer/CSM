using CSM.Extensions;

namespace CSM.Commands.Handler
{
    public class TaxRateChangeHandler : CommandHandler<TaxRateChangeCommand>
    {
        public override void Handle(TaxRateChangeCommand command)
        {
            command.TaxRate.CopyTo(EconomyExtension._LastTaxrate, 0);
            command.TaxRate.CopyTo(EconomyExtension._Taxrate, 0);
        }
    }
}
