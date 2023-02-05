using System;
using CitiesHarmony.API;
using ColossalFramework;
using ColossalFramework.IO;
using ColossalFramework.Threading;
using CSM.API;
using CSM.Commands;
using CSM.GS.Commands;
using CSM.Helpers;
using CSM.Injections;
using CSM.Mods;
using CSM.Panels;
using ICities;

namespace CSM
{
    public class CSM : IUserMod
    {
        public static Settings Settings;
        public static bool IsSteamPresent { get; private set; }

        public CSM()
        {
            // Setup the correct logging configuration
            Log.Instance = new Log(DataLocation.localApplicationData);

            Settings = new Settings();
            Log.Instance.LogDebug = Settings.DebugLogging.value;

            ModCompat.Init();

            CommandInternal.Instance = new CommandInternal();
            ApiCommand.Instance = new ApiCommand();
            ApiCommand.Instance.RefreshModel();
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
                MainMenuHandler.Init();

                IsSteamPresent = SteamHelpers.Init();
                if (IsSteamPresent)
                {
                    Log.Info("CSM is running in a steam context. Steam features will be enabled.");
                }
                else
                {
                    Log.Warn("CSM is not running in a steam context. Steam features will not be enabled.");
                }

                if (IsSteamPresent)
                {
                    if (Singleton<LoadingManager>.instance.m_loadingComplete)
                    {
                        // Already in-game
                    }
                    else if (Singleton<LoadingManager>.instance.m_currentlyLoading ||
                            !Singleton<LoadingManager>.instance.LoadingAnimationComponent.AnimationLoaded)
                    {
                        // Game not yet loaded, queue command line check
                        Singleton<LoadingManager>.instance.m_introLoaded += SteamHelpers.Instance.CheckCommandLineCallback;
                    }
                    else
                    {
                        // Game already loaded, we are in main menu, do command line check
                        if (ThreadHelper.dispatcher == Dispatcher.currentSafe)
                        {
                            SteamHelpers.Instance.CheckCommandLine(true);
                        }
                        else
                        {
                            ThreadHelper.dispatcher.Dispatch(() =>
                            {
                                SteamHelpers.Instance.CheckCommandLine(true);
                            });
                        }
                    }
                }
                Log.Info("Construction Complete!");
            });
        }

        public void OnDisabled()
        {
            Log.Info("Unpatching Harmony...");
            if (HarmonyHelper.IsHarmonyInstalled) Patcher.UnpatchAll();
            // Destroys all the connections made to external mods
            ModSupport.Instance.DestroyConnections();
            if (IsSteamPresent) SteamHelpers.Instance.Shutdown();
            Log.Info("Destruction complete!");
        }

        public void OnSettingsUI(UIHelperBase helper) => SettingsPanel.Build(helper, Settings);

        public string Name => "Cities: Skylines Multiplayer";

        public string Description => "Multiplayer mod for Cities: Skylines.";
    }
}
