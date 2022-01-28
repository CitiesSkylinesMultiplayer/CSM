using System.Reflection;
using HarmonyLib;

namespace CSM
{
    public static class Patcher
    {
        private static Harmony _harmony;

        public static void PatchAll()
        {
            _harmony = new Harmony("com.citiesskylinesmultiplayer");
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void UnpatchAll()
        {
            _harmony.UnpatchAll("com.citiesskylinesmultiplayer");
        }
    }
}
