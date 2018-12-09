using ColossalFramework;
using CSM.Networking;
using System.Reflection;

namespace CSM.Commands.Handler
{
    public class MoneyHandler : CommandHandler<MoneyCommand>
    {
        public override byte ID => CommandIds.MoneyCommand;

        public override void HandleOnClient(MoneyCommand command) => HandleClient(command);

        public override void HandleOnServer(MoneyCommand command, Player player) => HandleServer(command);

        private void HandleClient(MoneyCommand command)
        {
            typeof(EconomyManager).GetField("m_cashAmount", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, command.InternalMoneyAmount);
            typeof(EconomyManager).GetField("m_lastCashAmount", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, command.InternalMoneyAmount);
            typeof(EconomyManager).GetField("m_totalExpenses", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, command.TotalExpenses);
            typeof(EconomyManager).GetField("m_totalIncome", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, command.TotalIncome);
        }

        private void HandleServer(MoneyCommand command)
        {
            typeof(EconomyManager).GetField("m_cashAmount", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, command.InternalMoneyAmount);
            typeof(EconomyManager).GetField("m_lastCashAmount", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, command.InternalMoneyAmount);
        }
    }
}