using System;
using System.Reflection;
using HarmonyLib;
using ICities;
using NLog;

namespace SampleExternalMod
{
    public class SampleUserMod : IUserMod
    {        
        private static readonly Logger _logger = LogManager.GetLogger("CSM");
        private Harmony _harmony;
        
        public string Name
        {
            get { return "Sample External Mod"; }
        }

        public string Description
        {
            get { return "Adds Nothing"; }
        }

        public void OnEnabled()
        {
            try
            {
                _harmony = new Harmony("com.mytest");
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                _logger.Error("[TEST] Patching failed", ex);
            }

            _logger.Info("[TEST] Construction Complete!");
        }

        public void OnDisabled()
        {
            _logger.Info("[TEST] Unpatching Harmony...");
            _harmony.UnpatchAll();
            _logger.Info("[TEST] Destruction complete!");
        }
    }
}
