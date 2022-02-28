using System;
using CSM.API;
using HarmonyLib;

namespace SampleExternalMod
{
    public static class Patcher
    {
        private const string HarmonyPatchId = "com.citiesskylinesmultiplayer.sampleexternalmod";

        public static void PatchAll()
        {
            // Instead of patching with harmony, other ways of handling changes in the mod are possible!
            try
            {
                Harmony harmony = new Harmony(HarmonyPatchId);
                harmony.PatchAll(typeof(SampleExternalModSupport).Assembly);
                Log.Info("[CSM SampleExternalMod Support] Patched!");
            }
            catch (Exception)
            {
                Log.Info("[CSM SampleExternalMod Support] Patching failed");
            }
        }

        public static void UnpatchAll()
        {
            new Harmony(HarmonyPatchId).UnpatchAll(HarmonyPatchId);
        }
    }
}
