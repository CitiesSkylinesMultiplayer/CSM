using CSM.Commands;
using Harmony;
using System;
using CSM.Networking;
using static EconomyManager;

namespace CSM.Injections
{
    /// <summary>
    /// This removes 
    /// </summary>
    class EconomyHandler
    {
    }

    [HarmonyPatch(typeof(EconomyManager))]
    [HarmonyPatch("AddResource")]
    [HarmonyPatch(new Type[] { typeof(Resource), typeof(int), typeof(ItemClass.Service), typeof(ItemClass.SubService), typeof(ItemClass.Level), typeof(DistrictPolicies.Taxation) })]

    public class AddResource
    {
        public static bool Prefix(Resource resource, int amount, out int __result)
        {
            __result = amount;
            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
            {
                switch (resource)
                {
                    case Resource.RewardAmount:
                    case Resource.CitizenIncome:
                    case Resource.PrivateIncome:
                    case Resource.PublicIncome:
                    case Resource.TourismIncome:
                        {
                            amount = 0;
                            return false;
                        }
                    default:
                        {
                            Command.SendToAll(new MoneyCommand
                            {
                                MoneyAmount = amount,
                            });
                            break;
                        }
                }
            }           
            return true;
        }
    }

    [HarmonyPatch(typeof(EconomyManager))]
    [HarmonyPatch("AddPrivateIncome")]
    public class AddPrivateIncome
    {
        public static bool Prefix(ref int amount, int taxRate, out int __result)
        {
            __result = amount;

            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
            {
                
                return false;
            }
            return true;

        }
    }

    [HarmonyPatch(typeof(EconomyManager))]
    [HarmonyPatch("FetchResource")]
    [HarmonyPatch(new Type[] { typeof(Resource), typeof(int), typeof(ItemClass.Service), typeof(ItemClass.SubService), typeof(ItemClass.Level) })]
    public class FetchResource
    {
        public static bool Prefix(Resource resource, ref int amount, ref int __result )
        {
            {
                __result = amount;
                if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
                {
                    switch (resource)
                    {
                        case Resource.CitizenIncome:
                        case Resource.LoanPayment:
                        case Resource.Maintenance:
                        case Resource.PolicyCost:
                            {
                                amount = 0;
                                return false;
                            }
                        default:
                            {
                                Command.SendToAll(new MoneyCommand
                                {
                                    MoneyAmount = -amount,
                                });
                                break;
                            }
                    }
                }
                return true;
            }
          
        }
    }

}
