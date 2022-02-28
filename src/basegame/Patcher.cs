using System;
using CSM.API;
using HarmonyLib;

namespace CSM.BaseGame
{
    public static class Patcher
    {
        private const string HarmonyPatchId = "com.citiesskylinesmultiplayer.basegame";

        public static void PatchAll()
        {
            try
            {
                Harmony harmony = new Harmony(HarmonyPatchId);
                harmony.PatchAll(typeof(BaseGameConnection).Assembly);
                Log.Info("[CSM BaseGame] Patched!");
            }
            catch (Exception)
            {
                Log.Info("[CSM BaseGame] Patching failed");
            }
        }

        public static void UnpatchAll()
        {
            new Harmony(HarmonyPatchId).UnpatchAll(HarmonyPatchId);
        }
    }
}
