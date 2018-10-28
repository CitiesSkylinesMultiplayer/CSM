
using ColossalFramework;
using CSM.Networking;
using System.Reflection;

namespace CSM.Commands.Handler
{
    class MoneyHandler : CommandHandler<MoneyCommand>
    {
        public override byte ID => 102;

        public override void HandleOnClient(MoneyCommand command) => Handle(command);

        public override void HandleOnServer(MoneyCommand command, Player player) => Handle(command);

        private void Handle(MoneyCommand command)
        {
            typeof(EconomyManager).GetField("m_cashAmount", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, command.InternalMoneyAmount);
            typeof(EconomyManager).GetField("m_lastCashAmount", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, command.InternalMoneyAmount);
            typeof(EconomyManager).GetField("m_totalExpenses", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, command.TotalExpenses);
            typeof(EconomyManager).GetField("m_totalIncome", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, command.TotalIncome);
        }
    }
}
