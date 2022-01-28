using ICities;
using NLog;
using System;
using CitiesHarmony.API;

namespace SampleExternalMod
{
    public class SampleUserMod : IUserMod
    {
        private static readonly Logger _logger = LogManager.GetLogger("CSM");

        public string Name => "Sample External Mod";

        public string Description => "Adds Nothing";

        public void OnEnabled()
        {
            HarmonyHelper.DoOnHarmonyReady(() =>
            {
                try
                {
                    Patcher.PatchAll();

                    _logger.Info("[TEST] Construction Complete!");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "[TEST] Patching failed");
                }
            });
        }

        public void OnDisabled()
        {
            _logger.Info("[TEST] Unpatching Harmony...");
            if (HarmonyHelper.IsHarmonyInstalled) Patcher.UnpatchAll();
            _logger.Info("[TEST] Destruction complete!");
        }
    }
}
