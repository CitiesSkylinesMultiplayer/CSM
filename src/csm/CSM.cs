using System;
using CitiesHarmony.API;
using ColossalFramework.IO;
using CSM.API;
using CSM.Commands;
using CSM.Injections;
using CSM.Mods;
using CSM.Panels;
using ICities;

namespace CSM
{
    public class CSM : IUserMod
    {
        public static Settings Settings;

        public CSM()
        {
            // Setup the correct logging configuration
            Log.Instance = new Log(DataLocation.localApplicationData);

            Settings = new Settings();
            Log.Instance.LogDebug = Settings.DebugLogging.value;

            ModCompat.Init();

            CommandInternal.Instance = new CommandInternal();
        }

        public void OnEnabled()
        {
            Log.Info("Attempting to patch Cities: Skylines using Harmony...");
            HarmonyHelper.DoOnHarmonyReady(() => {
                try
                {
                    Patcher.PatchAll();
                    Log.Info("Successfully patched Cities: Skylines!");
                }
                catch (Exception ex)
                {
                    Log.Error("Patching failed", ex);
                }
                // Registers all other mods which implement the API
                ModSupport.Instance.Init();
                // Setup join button
                MainMenuHandler.CreateOrUpdateJoinGameButton();

                Log.Info("Construction Complete!");
            });
        }

        public void OnDisabled()
        {
            Log.Info("Unpatching Harmony...");
            if (HarmonyHelper.IsHarmonyInstalled) Patcher.UnpatchAll();
            // Destroys all the connections made to external mods
            ModSupport.Instance.DestroyConnections();
            Log.Info("Destruction complete!");
        }

        public void OnSettingsUI(UIHelperBase helper) => SettingsPanel.Build(helper, Settings);

        public string Name => "Cities: Skylines Multiplayer";

        public string Description => "Multiplayer mod for Cities: Skylines.";
    }
}
