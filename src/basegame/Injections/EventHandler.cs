using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework.UI;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Events;
using HarmonyLib;
using UnityEngine;

namespace CSM.BaseGame.Injections
{
    [HarmonyPatch(typeof(EventAI))]
    [HarmonyPatch("Activate")]
    public class ActivateEvent
    {
        public static void Prefix(ushort eventID)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            Command.SendToAll(new EventActivateCommand()
            {
                Event = eventID
            });
        }
    }

    [HarmonyPatch(typeof(EventAI))]
    [HarmonyPatch("SetColor")]
    public class RocketSetColor
    {
        public static void Prefix(ushort eventID, ref EventData data, Color32 newColor, EventAI __instance)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            Type type = __instance.GetType();
            if (type != typeof(RocketLaunchAI) && type != typeof(ConcertAI) && type != typeof(SportMatchAI) && type != typeof(VarsitySportsMatchAI))
                return;

            if (newColor.r == data.m_color.r && newColor.g == data.m_color.g && newColor.b == data.m_color.b)
                return;

            Command.SendToAll(new EventColorChangedCommand()
            {
                Event = eventID,
                Color = newColor
            });
        }
    }

    [HarmonyPatch(typeof(EventAI))]
    [HarmonyPatch("SetSecurityBudget")]
    public class SetSecurityBudget
    {
        public static void Prefix(ushort eventID, ref EventData data, int newBudget, EventAI __instance)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            if (__instance.GetSecurityBudget(eventID, ref data) == newBudget)
                return;

            Command.SendToAll(new EventSetSecurityBudgetCommand()
            {
                Event = eventID,
                Budget = newBudget
            });
        }
    }

    [HarmonyPatch(typeof(EventAI))]
    [HarmonyPatch("SetTicketPrice")]
    public class SetTicketPrice
    {
        public static void Prefix(ushort eventID, ref EventData data, int newPrice, EventAI __instance)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            // Event is 0, when it is set through the FestivalPanel for a certain band
            if (eventID == 0 || __instance.GetTicketPrice(eventID, ref data) == newPrice)
                return;

            Command.SendToAll(new EventSetTicketPriceCommand()
            {
                Event = eventID,
                Price = newPrice
            });
        }
    }

    [HarmonyPatch]
    public class BeginEvent
    {
        private static bool _isWinMatchSet;
        private static bool _winMatchSet;

        public static void Prefix(ref EventData data)
        {
            _isWinMatchSet = false;
            if (Command.CurrentRole == MultiplayerRole.Client)
            {
                bool matchSuccess = (data.m_flags & EventData.Flags.Success) != EventData.Flags.None;
                bool matchFail = (data.m_flags & EventData.Flags.Failure) != EventData.Flags.None;
                if (matchSuccess || matchFail)
                {
                    _isWinMatchSet = true;
                    _winMatchSet = matchSuccess;
                }
            }
        }

        public static void Postfix(ushort eventID, ref EventData data)
        {
            if (Command.CurrentRole == MultiplayerRole.Client)
            {
                // Override result of this method as we already have the result set from the server
                if (_isWinMatchSet)
                {
                    if (_winMatchSet)
                    {
                        data.m_flags |= EventData.Flags.Success;
                        data.m_flags &= ~EventData.Flags.Failure;
                    }
                    else
                    {
                        data.m_flags |= EventData.Flags.Failure;
                        data.m_flags &= ~EventData.Flags.Success;
                    }
                }
            }
            else
            {
                // Synchronize randomly chosen match result
                bool winMatch = (data.m_flags & EventData.Flags.Success) != EventData.Flags.None;
                Command.SendToAll(new EventSetResultCommand
                {
                    Event = eventID,
                    Result = winMatch
                });
            }
        }

        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return typeof(SportMatchAI).GetMethod("BeginEvent", ReflectionHelper.AllAccessFlags);
            yield return typeof(ConcertAI).GetMethod("BeginEvent", ReflectionHelper.AllAccessFlags);
        }
    }

    [HarmonyPatch(typeof(FestivalPanel))]
    [HarmonyPatch("OnTicketPriceChanged")]
    public class SetConcertTicketPrice
    {
        public static void Prefix(UIComponent comp, int value, InstanceID ___m_InstanceID)
        {
            ConcertAI concertAI = PrefabCollection<EventInfo>.GetLoaded((uint) comp.objectUserData).GetAI() as ConcertAI;
            int newPrice = value * 100;
            int num = newPrice - concertAI.m_ticketPrice;
            if (num == concertAI.m_info.m_ticketPriceOffset)
                return;

            Command.SendToAll(new EventSetConcertTicketPriceCommand()
            {
                Building = ___m_InstanceID.Building,
                Event = (uint) comp.objectUserData,
                Price = newPrice
            });
        }
    }
}
