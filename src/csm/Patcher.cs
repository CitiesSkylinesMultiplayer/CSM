using System.Reflection;
using HarmonyLib;

namespace CSM
{
    public static class Patcher
    {
        private const string HarmonyPatchID = "com.citiesskylinesmultiplayer";

        public static void PatchAll()
        {
            Harmony harmony = new Harmony(HarmonyPatchID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void UnpatchAll()
        {
            new Harmony(HarmonyPatchID).UnpatchAll(HarmonyPatchID);
        }
    }
}
