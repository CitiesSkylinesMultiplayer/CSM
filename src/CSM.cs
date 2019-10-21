using HarmonyLib;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Reflection;

namespace CSM
{
    public class CSM : ICities.IUserMod
    {
        private readonly Harmony _harmony;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public CSM()
        {
            // Setup the correct logging configuration
            SetupLogging();

            try
            {
                _logger.Info("Attempting to patch Cities: Skylines using Harmony...");
                _harmony = new Harmony("net.gridentertainment.csm");
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
                _logger.Info("Successfully patched Cities: Skylines!");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Patching failed");
            }

            _logger.Info("Construction Complete!");
        }

        ~CSM()
        {
            _logger.Info("Unpatching Harmony...");
            _harmony.UnpatchAll();
            _logger.Info("Destruction complete!");
        }

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

            // While in development set both levels to start at debug, later on we
            // want to set an option to do this.
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logConsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            LogManager.Configuration = config;
        }

        public string Name => "Cities: Skylines Multiplayer";

        public string Description => "Multiplayer mod for Cities: Skylines.";

        /// <summary>
        ///     Log a message to the console.
        /// </summary>
        /// <param name="message"></param>
        [Obsolete("Use NLog instead.")]
        public static void Log(string message)
        {
            _logger.Info(message);
        }
    }
}
