using System;
using CSM.API;
using CSM.Panels;
using HarmonyLib;
using ICities;

namespace CSM.Injections
{
    [HarmonyPatch(typeof(ChirpPanel))]
    [HarmonyPatch("Hide")]
    public class ChirpHide
    {
        public static void Prefix()
        {
            ((ChatLogPanel) Chat.Instance).HideChirpText();
        }
    }

    [HarmonyPatch(typeof(ChirpPanel))]
    [HarmonyPatch("AddMessage")]
    [HarmonyPatch(new Type[] { typeof(IChirperMessage) })]
    public class ChirperMessage
    {
        public static bool Prefix()
        {
            // Prevent printing default chirper messages
            return CSM.Settings.PrintChirperMsgs.value;
        }
    }
}
