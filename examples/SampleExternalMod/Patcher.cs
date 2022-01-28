using System.Reflection;
using HarmonyLib;

namespace SampleExternalMod
{
    public static class Patcher
    {
        private static Harmony _harmony;

        public static void PatchAll()
        {
            _harmony = new Harmony("com.mytest");
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void UnpatchAll()
        {
            _harmony.UnpatchAll("com.mytest");
        }
    }
}
