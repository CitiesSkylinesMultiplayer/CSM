using CSM.Injections;
using CSM.Panels;
using CSM.Util;
using HarmonyLib;
using ICities;
using System;
using System.Reflection;

namespace CSM
{
    public class CSM : ICities.IUserMod
    {
        private Harmony _harmony;

        public static Settings Settings;

        public CSM()
        {
            // Setup the correct logging configuration
            Settings = new Settings();
            Log.Initialize(Settings.DebugLogging.value);

            ModCompat.Init();
        }

        public void OnEnabled()
        {
            try
            {
                Log.Info("Attempting to patch Cities: Skylines using Harmony...");
                _harmony = new Harmony("com.citiesskylinesmultiplayer");
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
                Log.Info("Successfully patched Cities: Skylines!");

                Log.Info("Adding!");
            }
            catch (Exception ex)
            {
                Log.Error("Patching failed", ex);
            }

            MainMenuHandler.CreateOrUpdateJoinGameButton();

            Log.Info("Construction Complete!");
        }

        public void OnDisabled()
        {
            Log.Info("Unpatching Harmony...");
            _harmony.UnpatchAll();
            Log.Info("Destruction complete!");
        }

        public void OnSettingsUI(UIHelperBase helper) => SettingsPanel.Build(helper, Settings);

        public string Name => "Cities: Skylines Multiplayer";

        public string Description => "Multiplayer mod for Cities: Skylines.";
    }
}
