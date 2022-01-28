using System;
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
            if (type != typeof(RocketLaunchAI) && type != typeof(ConcertAI) && type != typeof(SportMatchAI))
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
            // TODO: Sync price changes in FestivalPanel and VarsitySportsArenaPanel
            if (eventID == 0 || __instance.GetTicketPrice(eventID, ref data) == newPrice)
                return;

            Command.SendToAll(new EventSetTicketPriceCommand()
            {
                Event = eventID,
                Price = newPrice
            });
        }
    }
}
