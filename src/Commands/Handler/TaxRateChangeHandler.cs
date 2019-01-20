using CSM.Extensions;

namespace CSM.Commands.Handler
{
    public class TaxRateChangeHandler : CommandHandler<TaxRateChangeCommand>
    {
        public override void Handle(TaxRateChangeCommand command)
        {
            command.Taxrate.CopyTo(EconomyExtension._LastTaxrate, 0);
            command.Taxrate.CopyTo(EconomyExtension._Taxrate, 0);
        }
    }
}
