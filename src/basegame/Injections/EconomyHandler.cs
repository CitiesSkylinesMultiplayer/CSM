using System;
using System.Collections.Generic;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Economy;
using HarmonyLib;
using static EconomyManager;

namespace CSM.BaseGame.Injections
{
    [HarmonyPatch(typeof(EconomyManager))]
    [HarmonyPatch("TakeNewLoan")]
    public class TakeNewLoan
    {
        public static void Prefix(int index, int amount, int interest, int length)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            Command.SendToAll(new EconomyTakeLoanCommand()
            {
                Index = index,
                Amount = amount,
                Interest = interest,
                Length = length
            });
        }
    }

    [HarmonyPatch(typeof(EconomyManager))]
    [HarmonyPatch("PayLoanNow")]
    public class PayLoanNow
    {
        public static void Prefix(int index)
        {
            Loan[] loans = ReflectionHelper.GetAttr<Loan[]>(EconomyManager.instance, "m_loans");

            if (IgnoreHelper.Instance.IsIgnored() || index >= loans.Length)
                return;

            int availMoney = EconomyManager.instance.PeekResource(Resource.LoanAmount, loans[index].m_amountLeft);

            Command.SendToAll(new EconomyPayLoanCommand()
            {
                Index = index,
                Paid = availMoney
            });
        }
    }

    [HarmonyPatch(typeof(EconomyManager))]
    [HarmonyPatch("AcceptBailout")]
    public class AcceptBailout
    {
        public static void Prefix()
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            Command.SendToAll(new EconomyBailoutCommand()
            {
                Accepted = true
            });
        }
    }

    [HarmonyPatch(typeof(EconomyManager))]
    [HarmonyPatch("RejectBailout")]
    public class RejectBailout
    {
        public static void Prefix()
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            Command.SendToAll(new EconomyBailoutCommand()
            {
                Accepted = false
            });
        }
    }

    [HarmonyPatch(typeof(EconomyManager))]
    [HarmonyPatch("SetBudget")]
    [HarmonyPatch(new Type[] { typeof(ItemClass.Service), typeof(ItemClass.SubService), typeof(int), typeof(bool) })]
    public class SetBudget
    {
        public static void Prefix(ItemClass.Service service, ItemClass.SubService subService, int budget, bool night, out bool __state)
        {
            if (IgnoreHelper.Instance.IsIgnored())
            {
                // Needed to prevent the postfix from recursively called methods from running
                __state = false;
                return;
            }

            __state = true;

            Command.SendToAll(new EconomySetBudgetCommand()
            {
                Service = service,
                SubService = subService,
                Budget = budget,
                Night = night
            });

            IgnoreHelper.Instance.StartIgnore("SetBudget");
        }

        public static void Postfix(ref bool __state)
        {
            if (!__state || IgnoreHelper.Instance.IsIgnored("SetBudget"))
                return;

            IgnoreHelper.Instance.EndIgnore("SetBudget");
        }
    }

    [HarmonyPatch(typeof(EconomyManager))]
    [HarmonyPatch("SetTaxRate")]
    [HarmonyPatch(new Type[] { typeof(ItemClass.Service), typeof(ItemClass.SubService), typeof(ItemClass.Level), typeof(int) })]
    public class SetTaxRate
    {
        public static void Prefix(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, int rate, out bool __state)
        {
            if (IgnoreHelper.Instance.IsIgnored())
            {
                // Needed to prevent the postfix from recursively called methods from running
                __state = false;
                return;
            }

            __state = true;

            Command.SendToAll(new EconomySetTaxRateCommand()
            {
                Service = service,
                SubService = subService,
                Level = level,
                Rate = rate
            });

            IgnoreHelper.Instance.StartIgnore("SetTaxRate");
        }

        public static void Postfix(ref bool __state)
        {
            if (!__state || IgnoreHelper.Instance.IsIgnored("SetTaxRate"))
                return;

            IgnoreHelper.Instance.EndIgnore("SetTaxRate");
        }
    }

    // Approach for syncing resources (e.g. money)
    // Fetch the resource where it's initially fetched (e.g. building create) (and send changed amount over)
    // Prevent the other side from fetching it again (e.g. in the building create handler)!
    // Ignore some types on the clients as they are fetched by the simulation tick
    // (Only fetch them on the server and send changed amount over)

    [HarmonyPatch(typeof(EconomyManager))]
    [HarmonyPatch("FetchResource")]
    [HarmonyPatch(new Type[] { typeof(Resource), typeof(int), typeof(ItemClass.Service), typeof(ItemClass.SubService), typeof(ItemClass.Level) })]
    public class FetchResource
    {
        public static bool DontFetchResource = false;
        public static bool DontRunPatch = false;
        public static int ReturnFetchedAmount = -1;

        public static bool Prefix(Resource resource, int amount, ref int __result, out bool __state)
        {
            __state = false;

            // Don't run patch when we are applying from the command handler
            if (DontRunPatch)
                return true;

            // Flag to prevent removing resources when it's not possible in the respective command handler
            if (DontFetchResource)
            {
                // Return a specific fetched amount to the caller of the FetchResource method
                if (ReturnFetchedAmount > -1)
                {
                    __result = ReturnFetchedAmount;
                    ReturnFetchedAmount = -1;
                }
                else
                {
                    __result = amount;
                }

                // Don't run actual method
                return false;
            }

            if (amount == 0)
                return true;

            // Check for resource types that only change in the simulation tick (ignore them in the client)
            if (Command.CurrentRole == MultiplayerRole.Client)
            {
                switch (resource)
                {
                    case Resource.CitizenIncome:
                    case Resource.LoanPayment:
                    case Resource.Maintenance:
                    case Resource.PolicyCost:
                    case Resource.ResourcePrice:
                    case Resource.FeePayment: // <-- Unsure if we need to sync this, maybe consider later
                        // Tell the method caller that the money was removed successfully
                        __result = amount;
                        return false;
                }
            }

            __state = true;
            return true;
        }

        public static void Postfix(Resource resource, ItemClass.Service service, ItemClass.SubService subService,
            ItemClass.Level level, ref int __result, ref bool __state)
        {
            if (!__state)
                return;

            ResourceCommandHandler.Queue(new EconomyResourceCommand()
            {
                Action = ResourceAction.FETCH,
                ResourceType = resource,
                ResourceAmount = __result,
                Service = service,
                SubService = subService,
                Level = level
            });
        }
    }

    [HarmonyPatch(typeof(EconomyManager))]
    [HarmonyPatch("AddResource")]
    [HarmonyPatch(new Type[] { typeof(Resource), typeof(int), typeof(ItemClass.Service), typeof(ItemClass.SubService), typeof(ItemClass.Level), typeof(DistrictPolicies.Taxation) })]
    public class AddResource
    {
        public static bool DontAddResource = false;
        public static bool DontRunPatch = false;

        public static bool Prefix(Resource resource, int amount, ItemClass.Service service, ItemClass.SubService subService,
            ItemClass.Level level, DistrictPolicies.Taxation taxationPolicies, ref int __result, out bool __state)
        {
            __state = false;

            // Always ignore the private income handler if it's called from AddResource
            AddPrivateIncome.DontRunPatch = true;

            // Don't run patch when we are applying from the command handler
            if (DontRunPatch)
                return true;

            // Flag to prevent adding resources when it's not possible in the respective command handler
            if (DontAddResource)
            {
                __result = amount;
                // Don't run actual method
                return false;
            }

            if (amount == 0)
                return true;

            // Check for resource types that only change in the simulation tick (ignore them in the client)
            if (Command.CurrentRole == MultiplayerRole.Client)
            {
                switch (resource)
                {
                    case Resource.RewardAmount:
                    case Resource.CitizenIncome:
                    case Resource.PublicIncome:
                    case Resource.TourismIncome:
                    case Resource.ResourcePrice:
                        // Tell the method caller that the money was added successfully
                        __result = amount;
                        return false;

                    case Resource.PrivateIncome:
                        // Return the same amount as the real method would return (see EconomyManager::AddPrivateIncome)
                        int taxRate = EconomyManager.instance.GetTaxRate(service, subService, level, taxationPolicies);
                        taxRate = UniqueFacultyAI.IncreaseByBonus(UniqueFacultyAI.FacultyBonus.Economics, taxRate);
                        int taxMultiplier = ReflectionHelper.GetAttr<int>(EconomyManager.instance, "m_taxMultiplier");
                        __result = (int)((amount * taxRate * taxMultiplier + 999999L) / 1000000L);
                        return false;
                }
            }

            __state = true;
            return true;
        }

        public static void Postfix(Resource resource, ItemClass.Service service, ItemClass.SubService subService,
            ItemClass.Level level, DistrictPolicies.Taxation taxationPolicies, int amount, ref bool __state)
        {
            AddPrivateIncome.DontRunPatch = false;

            if (!__state)
                return;

            ResourceCommandHandler.Queue(new EconomyResourceCommand()
            {
                Action = ResourceAction.ADD,
                ResourceType = resource,
                ResourceAmount = amount,
                Service = service,
                SubService = subService,
                Level = level,
                Taxation = taxationPolicies
            });
        }
    }

    // This method is mostly called by AddResource (we ignore it in this case), but also sometimes directly.
    [HarmonyPatch(typeof(EconomyManager))]
    [HarmonyPatch("AddPrivateIncome")]
    public class AddPrivateIncome
    {
        public static bool DontRunPatch = false;

        public static bool Prefix(int amount, ItemClass.Service service, ItemClass.SubService subService,
            ItemClass.Level level, int taxRate, ref int __result)
        {
            if (DontRunPatch)
                return true;

            // This method is only called from the simulation tick, so only execute it on the server
            if (Command.CurrentRole == MultiplayerRole.Client)
            {
                __result = amount;
                return false;
            }

            ResourceCommandHandler.Queue(new EconomyResourceCommand()
            {
                Action = ResourceAction.PRIVATE,
                ResourceAmount = amount,
                Service = service,
                SubService = subService,
                Level = level,
                TaxRate = taxRate
            });
            return true;
        }
    }

    /// <summary>
    ///     Class to buffer EconomyResourceCommands to prevent packet spam.
    ///     Queued packets are sent by the ThreadingExtension every 2 seconds.
    /// </summary>
    public static class ResourceCommandHandler
    {
        private static readonly List<EconomyResourceCommand> _commands = new List<EconomyResourceCommand>();

        public static void Queue(EconomyResourceCommand cmd)
        {
            _commands.Add(cmd);
        }

        public static void Send()
        {
            if (_commands.Count == 0)
                return;

            EconomyResourceCommand[] cmdArray = _commands.ToArray();

            Command.SendToAll(new EconomyResourcesCommand()
            {
                Commands = cmdArray
            });

            _commands.Clear();
        }
    }
}
