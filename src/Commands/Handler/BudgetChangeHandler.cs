using CSM.Extensions;

namespace CSM.Commands.Handler
{
    public class BudgetChangeHandler : CommandHandler<BudgetChangeCommand>
    {
        public override void Handle(BudgetChangeCommand command)
        {
            command.ServiceBudgetNight.CopyTo(EconomyExtension._LastserviceBudgetNight, 0);
            command.ServiceBudgetNight.CopyTo(EconomyExtension._serviceBudgetNight, 0);
            command.ServiceBudgetDay.CopyTo(EconomyExtension._LastserviceBudgetDay, 0);
            command.ServiceBudgetDay.CopyTo(EconomyExtension._serviceBudgetDay, 0);
        }
    }
}
