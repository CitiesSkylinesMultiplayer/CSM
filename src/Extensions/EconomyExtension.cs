using ColossalFramework;
using CSM.Commands;
using CSM.Networking;
using ICities;
using System.Reflection;

namespace CSM.Extensions
{
    /// <summary>
    ///     Handles game economy. Sends the MoneyAmount between Server and Client
    ///     sets the income and the expenses to 0 on the client side, making the server handling income and expenses
	///     Sync the total income and expences on the UI
    /// </summary>
    public class EconomyExtension : EconomyExtensionBase
    {
        private long _lastMoneyAmount;
        //private int[] _taxrate;
        //private int[] _serviceBudgetNight;
        //private int[] _serviceBudgetDay;

        public override long OnUpdateMoneyAmount(long internalMoneyAmount) //function that checks if the money updates
        {
            if (_lastMoneyAmount != (long) typeof(EconomyManager).GetField("m_cashAmount", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance))
            {
                switch (MultiplayerManager.Instance.CurrentRole)
                {
                    case MultiplayerRole.Client:
                        if (_lastMoneyAmount != (long) typeof(EconomyManager).GetField("m_cashAmount", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance))
                            Command.SendToServer(new MoneyCommand
                            {
                                InternalMoneyAmount = (long) typeof(EconomyManager).GetField("m_cashAmount", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance)
                            });




                        break;

                    case MultiplayerRole.Server:
                        if (_lastMoneyAmount != (long) typeof(EconomyManager).GetField("m_cashAmount", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance))
                            Command.SendToClients(new MoneyCommand
                            {
                                InternalMoneyAmount = (long) typeof(EconomyManager).GetField("m_cashAmount", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance),
                                TotalExpenses = (long[]) typeof(EconomyManager).GetField("m_totalExpenses", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance),
                                TotalIncome = (long[]) typeof(EconomyManager).GetField("m_totalIncome", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance)

                            });
                        break;
                }
            }

            _lastMoneyAmount = (long) typeof(EconomyManager).GetField("m_cashAmount", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance);
            return (internalMoneyAmount);
        }

        public override int OnAddResource(EconomyResource resource, int amount, Service service, SubService subService, Level level)
        {
            switch (MultiplayerManager.Instance.CurrentRole)
            {
                case MultiplayerRole.Client:
                    {
                        typeof(EconomyManager).GetField("m_taxMultiplier", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Singleton<EconomyManager>.instance, 0);
                        break;
                    }
            }
            return amount;
        }

        public override int OnGetMaintenanceCost(int originalMaintenanceCost, Service service, SubService subService, Level level)
        {
            switch (MultiplayerManager.Instance.CurrentRole)
            {
                case MultiplayerRole.Client:
                    {
                        return 0;
                    }
                case MultiplayerRole.Server:
                    {
                        return originalMaintenanceCost;
                    }
            }
            return originalMaintenanceCost;
        }
    }
}
