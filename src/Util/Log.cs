using ColossalFramework.UI;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Reflection;

namespace CSM.Util
{
    class Log
    {
        private static readonly Logger _logger = LogManager.GetLogger("CSM");

        public static void Initialize(bool debug)
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

            // Target for console logging
            //MethodCallTarget logDebugConsole = new MethodCallTarget("logdebugconsole", (logEvent, o) =>
            //{
            //    if (logEvent.Level == LogLevel.Error)
            //        DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Error, logEvent.FormattedMessage);
            //    else if (logEvent.Level == LogLevel.Warn)
            //        DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Warning, logEvent.FormattedMessage);
            //    else
            //        DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, logEvent.FormattedMessage);
            //});

            // If debug logging is enabled
            if (debug)
            {
                //config.AddRule(LogLevel.Debug, LogLevel.Fatal, logDebugConsole);
                config.AddRule(LogLevel.Debug, LogLevel.Fatal, logConsole);
                config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            }
            else
            {
                //config.AddRule(LogLevel.Info, LogLevel.Fatal, logDebugConsole);
                config.AddRule(LogLevel.Info, LogLevel.Fatal, logConsole);
                config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
            }

            LogManager.Configuration = config;
        }

        public static void ChangeLogLevel(LogLevel level)
        {
            LogManager.Configuration.LoggingRules.ForEach(x => x.SetLoggingLevels(level, LogLevel.Fatal));
            LogManager.ReconfigExistingLoggers();
        }

        public static void Error(string message)
        {
            _logger.Error(message);
        }

        public static void Error(string message, Exception ex)
        {
            _logger.Error(ex, message);
        }

        public static void Warn(string message)
        {
            _logger.Warn(message);
        }

        public static void Info(string message)
        {
            _logger.Info(message);
        }

        public static void Debug(string message)
        {
            _logger.Debug(message);
        }
    }
}
