using System;
using System.Reflection;
using CSM.API;
using HarmonyLib;

namespace CSM.BaseGame
{
    public class BaseGameConnection : Connection
    {
        private const string HarmonyPatchID = "com.citiesskylinesmultiplayer.basegame";

        public BaseGameConnection()
        {
            Name = "Cities: Skylines";
            Enabled = true;
            try
            {
                Harmony harmony = new Harmony(HarmonyPatchID);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception)
            {
                Log.Info("[CSM BaseGame] Patching failed");
            }
        }

        ~BaseGameConnection()
        {
            new Harmony(HarmonyPatchID).UnpatchAll(HarmonyPatchID);
        }
    }
}
