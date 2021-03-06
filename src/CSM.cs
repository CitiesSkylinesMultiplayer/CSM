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

        private readonly Settings _settings;

        public static bool IsSteamPresent { get; private set; }

        public CSM()
        {
            // Setup the correct logging configuration
            _settings = new Settings();
            Log.Initialize(_settings.DebugLogging.value);
        }

        public void OnEnabled()
        {
            try
            {
                Log.Info("Attempting to patch Cities: Skylines using Harmony...");
                _harmony = new Harmony("com.citiesskylinesmultiplayer");
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
                Log.Info("Successfully patched Cities: Skylines!");
            }
            catch (Exception ex)
            {
                Log.Error("Patching failed", ex);
            }

            MainMenuHandler.CreateOrUpdateJoinGameButton();

            try
            {
                IsSteamPresent = SteamHelpers.Init();
                if (IsSteamPresent)
                {
                    Log.Info("Mod is running in a steam context. Steam features will be enabled.");
                }
                else
                {
                    Log.Error("Steam API init returned false! Steam features will not be enabled.");
                }
            }
            catch (Exception e)
            {
                Log.Error("Error running Steam API init: " + e.ToString());
                IsSteamPresent = false;
            }

            Log.Info("Construction Complete!");
        }

        public void OnDisabled()
        {
            Log.Info("Unpatching Harmony...");
            _harmony.UnpatchAll();
            Log.Info("Destruction complete!");
        }

        public void OnSettingsUI(UIHelperBase helper) => SettingsPanel.Build(helper, _settings);

        public string Name => "Cities: Skylines Multiplayer";

        public string Description => "Multiplayer mod for Cities: Skylines.";
    }
}
