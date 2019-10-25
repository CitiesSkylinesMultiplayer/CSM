using CSM.Commands.Data.Money;
using CSM.Extensions;

namespace CSM.Commands.Handler.Money
{
    public class BudgetChangeHandler : CommandHandler<BudgetChangeCommand>
    {
        protected override void Handle(BudgetChangeCommand command)
        {
            command.ServiceBudgetNight.CopyTo(EconomyExtension._LastserviceBudgetNight, 0);
            command.ServiceBudgetNight.CopyTo(EconomyExtension._serviceBudgetNight, 0);
            command.ServiceBudgetDay.CopyTo(EconomyExtension._LastserviceBudgetDay, 0);
            command.ServiceBudgetDay.CopyTo(EconomyExtension._serviceBudgetDay, 0);
        }
    }
}
