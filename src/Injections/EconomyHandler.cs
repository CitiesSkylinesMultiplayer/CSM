using ColossalFramework;
using CSM.Commands;
using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using CSM.Networking;
using static EconomyManager;
using ICities;


namespace CSM.Injections
{
    class EconomyHandler
    {
    }

    [HarmonyPatch(typeof(EconomyManager))]
    [HarmonyPatch("AddResource")]
    [HarmonyPatch(new Type[] { typeof(Resource), typeof(int), typeof(ItemClass.Service), typeof(ItemClass.SubService), typeof(ItemClass.Level), typeof(DistrictPolicies.Taxation) })]

    public class AddResource
    {
        public static void Prefix(Resource resource, int amount, out object __state)
        {
            __state = amount;
            //if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
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
                            break;
                        }

                }
            }
            UnityEngine.Debug.Log($"{resource} added, amount: {amount}");
        }
    }

    [HarmonyPatch(typeof(EconomyManager))]
    [HarmonyPatch("AddResource")]
    [HarmonyPatch(new Type[] { typeof(Resource), typeof(int), typeof(ItemClass.Service), typeof(ItemClass.SubService), typeof(ItemClass.Level), typeof(DistrictPolicies.Taxation) })]

    public class AddResourcePostfix
    {
        public static void Postfix(int __state)
        {
            UnityEngine.Debug.Log($"postfix løber");
        }
    }


    [HarmonyPatch(typeof(EconomyManager))]
    [HarmonyPatch("FetchResource")]
    [HarmonyPatch(new Type[] { typeof(Resource), typeof(int), typeof(ItemClass.Service), typeof(ItemClass.SubService), typeof(ItemClass.Level) })]
    public class FetchResource
    {
        public static void Prefix(Resource resource, ref int amount)
        {
            //if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
            {
            
                switch (resource)
                {
                    case Resource.CitizenIncome:
                    case Resource.LoanPayment:
                    case Resource.Maintenance:
                    case Resource.PolicyCost:
                        {
                        amount = 0;
                        //EconomyManager.instance.AddResource(Resource.RefundAmount, amount, ItemClass.Service.None, ItemClass.SubService.None,ItemClass.Level.None,DistrictPolicies.Taxation.None);
                        break;
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
            UnityEngine.Debug.Log($"{resource} fetched, amount {amount}");

            //if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
            //{
            //    UnityEngine.Debug.Log("virker stadig");
            //}
          
        }
    }

}
