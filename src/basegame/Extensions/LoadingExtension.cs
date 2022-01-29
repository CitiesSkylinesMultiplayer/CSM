using System;
using CSM.API;
using HarmonyLib;
using ICities;

namespace CSM.BaseGame.Extensions
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private const string HarmonyPatchID = "com.citiesskylinesmultiplayer.basegame";

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            try
            {
                Harmony harmony = new Harmony(HarmonyPatchID);
                harmony.PatchAll(typeof(BaseGameConnection).Assembly);
                Log.Info("[CSM BaseGame] Patched!");
            }
            catch (Exception)
            {
                Log.Info("[CSM BaseGame] Patching failed");
            }
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            new Harmony(HarmonyPatchID).UnpatchAll(HarmonyPatchID);
        }
    }
}
