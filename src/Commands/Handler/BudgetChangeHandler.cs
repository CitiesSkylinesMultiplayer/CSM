using System.Reflection;
using ColossalFramework;
using CSM.Extensions;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class BudgetChangeHandler : CommandHandler<BudgetChangeCommand>
    {
        public override byte ID => 108;

        public override void HandleOnServer(BudgetChangeCommand command, Player player) => HandleBudget(command);

        public override void HandleOnClient(BudgetChangeCommand command) => HandleBudget(command);

        private void HandleBudget(BudgetChangeCommand command)
        {
            command.ServiceBudgetNight.CopyTo(EconomyExtension._LastserviceBudgetNight, 0);
            command.ServiceBudgetNight.CopyTo(EconomyExtension._serviceBudgetNight, 0);
            command.ServiceBudgetDay.CopyTo(EconomyExtension._LastserviceBudgetDay, 0);
            command.ServiceBudgetDay.CopyTo(EconomyExtension._serviceBudgetDay, 0);
        }
    }
}