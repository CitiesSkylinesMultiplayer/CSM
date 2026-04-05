using System;
using System.Collections.Generic;
using System.Reflection;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Events;
using HarmonyLib;

namespace CSM.BaseGame.Injections
{
    /// <summary>
    ///     Harmony patches for Race Day DLC synchronization.
    ///     Handles: StartRace, EndRace, Cancel for all RaceEventAI subclasses.
    /// </summary>

    [HarmonyPatch(typeof(RaceEventAI))]
    [HarmonyPatch("StartRace")]
    public class RaceEventStart
    {
        public static void Prefix(ushort eventID)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            Log.Info($"[CSM Race] StartRace intercepted for event {eventID}");

            Command.SendToAll(new RaceEventStartCommand()
            {
                Event = eventID
            });
        }
    }

    [HarmonyPatch(typeof(RaceEventAI))]
    [HarmonyPatch("EndRace")]
    public class RaceEventEnd
    {
        public static void Prefix(ushort eventID)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            Log.Info($"[CSM Race] EndRace intercepted for event {eventID}");

            Command.SendToAll(new RaceEventEndCommand()
            {
                Event = eventID
            });
        }
    }

    [HarmonyPatch(typeof(RaceEventAI))]
    [HarmonyPatch("Cancel")]
    public class RaceEventCancel
    {
        public static void Prefix(ushort eventID, ref EventData data, RaceEventAI __instance)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            // Only sync if the event is not already completed or cancelled
            if ((data.m_flags & (EventData.Flags.Completed | EventData.Flags.Cancelled)) != 0)
                return;

            Log.Info($"[CSM Race] Cancel intercepted for event {eventID}");

            Command.SendToAll(new RaceEventCancelCommand()
            {
                Event = eventID
            });
        }
    }


}
