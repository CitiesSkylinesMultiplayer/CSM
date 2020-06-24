using CSM.Injections;
using CSM.Panels;
using HarmonyLib;
using ICities;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Reflection;
#if DEBUG
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Text;
#endif
namespace CSM
{
    public class CSM : ICities.IUserMod
    {
        private Harmony _harmony;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly Settings _settings;

#if DEBUG
        // Imports for the debug console
        [DllImport("kernel32.dll",
            EntryPoint = "GetStdHandle",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll",
            EntryPoint = "AllocConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();
#endif

        public CSM()
        {
            // Setup the correct logging configuration
            _settings = new Settings();
            SetupLogging();

#if DEBUG
            // This will output all logs to the console instead of a file. (Build with Debug mode on)
            // This will not be shipped in a release build because of the #if DEBUG tags
            AllocConsole();
            IntPtr stdHandle = GetStdHandle(-11);
            SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            Encoding encoding = System.Text.Encoding.GetEncoding(System.Text.Encoding.UTF8.CodePage);
            StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
#endif

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
