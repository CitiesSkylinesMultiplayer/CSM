using CSM.Injections;
using CSM.Panels;
using HarmonyLib;
using ICities;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Reflection;

namespace CSM
{
    public class CSM : ICities.IUserMod
    {
        private Harmony _harmony;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly Settings _settings;

        public CSM()
        {
            // Setup the correct logging configuration
            _settings = new Settings();
            SetupLogging();
        }

        public void OnEnabled()
        {
            try
            {
                _logger.Info("Attempting to patch Cities: Skylines using Harmony...");
                _harmony = new Harmony("com.citiesskylinesmultiplayer");
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
                _logger.Info("Successfully patched Cities: Skylines!");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Patching failed");
            }

            MainMenuHandler.CreateOrUpdateJoinGameButton();

            _logger.Info("Construction Complete!");
        }

        public void OnDisabled()
        {
            _logger.Info("Unpatching Harmony...");
            _harmony.UnpatchAll();
            _logger.Info("Destruction complete!");
        }

        public void OnSettingsUI(UIHelperBase helper) => SettingsPanel.Build(helper, _settings);

        /// <summary>
        ///     This code sets up the different
        ///     logging levels for the mod.
        /// </summary>
        private void SetupLogging()
        {
            LoggingConfiguration config = new LoggingConfiguration();

            // The layout of the log
            string layout = "[${time}] [" + Assembly.GetAssembly(typeof(CSM)).GetName().Version + "] [${level}] ${message} ${exception:format=tostring}";

            // Target for file logging
            FileTarget logfile = new FileTarget("logfile")
            {
                FileName = "multiplayer-logs/log-current.txt",
                ArchiveFileName = "multiplayer-logs/log-${shortdate}.txt",
                Layout = layout,
                ArchiveEvery = FileArchivePeriod.Day,
                MaxArchiveFiles = 7,
                ConcurrentWrites = true,
            };

            // Target for console logging
            ConsoleTarget logConsole = new ConsoleTarget("logconsole") { Layout = layout };

            // If debug logging is enabled
            if (_settings.DebugLogging.value)
            {
                config.AddRule(LogLevel.Debug, LogLevel.Fatal, logConsole);
                config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            }
            else
            {
                config.AddRule(LogLevel.Info, LogLevel.Fatal, logConsole);
                config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
            }

            LogManager.Configuration = config;
        }

        public string Name => "Cities: Skylines Multiplayer";

        public string Description => "Multiplayer mod for Cities: Skylines.";
    }
}
