using System.Reflection;
using ColossalFramework;
using CSM.Commands.Data.Money;
using CSM.Networking;

namespace CSM.Commands.Handler.Money
{
    public class MoneyHandler : CommandHandler<MoneyCommand>
    {
        public MoneyHandler()
        {
            RelayOnServer = false;
        }

        protected override void Handle(MoneyCommand command)
        {
            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
            {
                long __cashAmount = (long)typeof(EconomyManager).GetField("m_cashAmount", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance);
                __cashAmount += command.MoneyAmount;
                typeof(EconomyManager).GetField("m_cashAmount", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, __cashAmount);
                typeof(EconomyManager).GetField("m_lastCashAmount", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, __cashAmount);
            }

            // Only on the client
            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
            {
                typeof(EconomyManager).GetField("m_cashAmount", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, command.MoneyAmount);
                typeof(EconomyManager).GetField("m_lastCashAmount", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, command.MoneyAmount);
                typeof(EconomyManager).GetField("m_totalExpenses", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, command.TotalExpenses);
                typeof(EconomyManager).GetField("m_totalIncome", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, command.TotalIncome);
            }
        }
    }
}
