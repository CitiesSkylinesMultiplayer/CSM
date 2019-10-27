using CSM.Commands.Data.Money;
using CSM.Extensions;

namespace CSM.Commands.Handler.Money
{
    public class TaxRateChangeHandler : CommandHandler<TaxRateChangeCommand>
    {
        protected override void Handle(TaxRateChangeCommand command)
        {
            command.TaxRate.CopyTo(EconomyExtension._LastTaxrate, 0);
            command.TaxRate.CopyTo(EconomyExtension._Taxrate, 0);
        }
    }
}
