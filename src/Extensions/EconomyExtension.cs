using ColossalFramework;
using CSM.Commands;
using CSM.Networking;
using ICities;
using System.Linq;
using System.Reflection;

namespace CSM.Extensions
{
    /// <summary>
    ///     Handles game economy. Sends the MoneyAmount between Server and Client
    ///     sets the income and the expenses to 0 on the client side, making the server handling income and expenses
    ///     Sync the total income and expenses on the UI
    ///     sync the underlying tax rate and service budget
    ///
    ///		TODO Sync the EconomyPanel
    /// </summary>
    public class EconomyExtension : EconomyExtensionBase
    {
        private long _lastMoneyAmount;
        private long[] _totalExpenses;
        private long[] _totalIncome;

        public static int[] _Taxrate;
        public static int[] _serviceBudgetNight;
        public static int[] _serviceBudgetDay;
        public static int[] _LastTaxrate = new int[120];
        public static int[] _LastserviceBudgetNight = new int[30];
        public static int[] _LastserviceBudgetDay = new int[30];

        public override void OnCreated(IEconomy economy)
        {
            _totalExpenses = (long[])typeof(EconomyManager).GetField("m_totalExpenses", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance);
            _totalIncome = (long[])typeof(EconomyManager).GetField("m_totalIncome", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance);
            _Taxrate = (int[])typeof(EconomyManager).GetField("m_taxRates", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance);
            _serviceBudgetDay = (int[])typeof(EconomyManager).GetField("m_serviceBudgetDay", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance);
            _serviceBudgetNight = (int[])typeof(EconomyManager).GetField("m_serviceBudgetNight", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance);

            _serviceBudgetNight.CopyTo(_LastserviceBudgetNight, 0);
            _serviceBudgetDay.CopyTo(_LastserviceBudgetDay, 0);
            _Taxrate.CopyTo(_LastTaxrate, 0);
        }

        public override long OnUpdateMoneyAmount(long internalMoneyAmount) //function that checks if the money updates
        {
            switch (MultiplayerManager.Instance.CurrentRole)
            {
                case MultiplayerRole.Client:
                    if (_lastMoneyAmount != (long)typeof(EconomyManager).GetField("m_cashAmount", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance))
                    {
                        Command.SendToServer(new MoneyCommand
                        {
                            InternalMoneyAmount = (long)typeof(EconomyManager).GetField("m_cashAmount", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance)
                        });
                    }

                    if (!_LastserviceBudgetDay.SequenceEqual(_serviceBudgetDay) | !_LastserviceBudgetNight.SequenceEqual(_serviceBudgetNight))
                    {
                        Command.SendToServer(new BudgetChangeCommand
                        {
                            ServiceBudgetDay = _serviceBudgetDay,
                            ServiceBudgetNight = _serviceBudgetNight
                        });
                        _serviceBudgetNight.CopyTo(_LastserviceBudgetNight, 0);
                        _serviceBudgetDay.CopyTo(_LastserviceBudgetDay, 0);
                    }

                    if (!_LastTaxrate.SequenceEqual(_Taxrate))
                    {
                        Command.SendToServer(new TaxRateChangeCommand
                        {
                            Taxrate = _Taxrate,
                        });

                        _Taxrate.CopyTo(_LastTaxrate, 0);
                    }

                    break;

                case MultiplayerRole.Server:
                    if (_lastMoneyAmount != (long)typeof(EconomyManager).GetField("m_cashAmount", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance))
                    {
                        Command.SendToClients(new MoneyCommand
                        {
                            InternalMoneyAmount = (long)typeof(EconomyManager).GetField("m_cashAmount", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance),
                            TotalExpenses = (long[])typeof(EconomyManager).GetField("m_totalExpenses", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance),
                            TotalIncome = (long[])typeof(EconomyManager).GetField("m_totalIncome", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance)
                        });
                    }

                    if (!_LastserviceBudgetDay.SequenceEqual(_serviceBudgetDay) | !_LastserviceBudgetNight.SequenceEqual(_serviceBudgetNight))
                    {
                        Command.SendToClients(new BudgetChangeCommand
                        {
                            ServiceBudgetDay = _serviceBudgetDay,
                            ServiceBudgetNight = _serviceBudgetNight
                        });
                        _serviceBudgetNight.CopyTo(_LastserviceBudgetNight, 0);
                        _serviceBudgetDay.CopyTo(_LastserviceBudgetDay, 0);
                    }

                    if (!_LastTaxrate.SequenceEqual(_Taxrate))
                    {
                        Command.SendToClients(new TaxRateChangeCommand
                        {
                            Taxrate = _Taxrate,
                        });

                        _Taxrate.CopyTo(_LastTaxrate, 0);
                    }
                    break;
            }

            _lastMoneyAmount = (long)typeof(EconomyManager).GetField("m_cashAmount", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Singleton<EconomyManager>.instance);
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