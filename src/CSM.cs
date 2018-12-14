using ColossalFramework.UI;
using CSM.Panels;
using Harmony;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Reflection;
using UnityEngine;

namespace CSM
{
    public class CSM : ICities.IUserMod
    {
        private readonly HarmonyInstance _harmony;

        private static readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        public CSM()
        {
            // Setup the correct logging configuration
            SetupLogging();

            // Make sure the button is enabled after intro load
            LoadingManager.instance.m_introLoaded += () => CreateJoinGameButton();

            // We are in the main menu, so load the "Join Game" button.
            if (!LoadingManager.instance.m_essentialScenesLoaded)
                CreateJoinGameButton();

            try
            {
                _logger.Info("Attempting to match Cities: Skylines using Harmony...");
                _harmony = HarmonyInstance.Create("net.gridentertainment.csm");
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
                _logger.Info("Successfully patches Cities: Skylines!");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        /// <summary>
        ///     This code sets up the different
        ///     logging levels for the mod.
        /// </summary>
        private void SetupLogging()
        {
            var config = new LoggingConfiguration();

            // The layout of the log
            var layout = "[${time}] [version] [${level}] ${message}";

            // Target for file logging
            var logfile = new FileTarget("logfile")
            {
                FileName = "multiplayer-logs/log-current.txt",
                ArchiveFileName = "multiplayer-logs/log-${shortdate}.txt",
                Layout = layout,
                ArchiveEvery = FileArchivePeriod.Day,
                MaxArchiveFiles = 7,
                ConcurrentWrites = true,
            };

            // Target for console logging
            var logconsole = new ConsoleTarget("logconsole") { Layout = layout };

            // While in development set both levels to start at debug, later on we
            // want to set an option to do this.
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            LogManager.Configuration = config;
        }

        ~CSM()
        {
            _harmony.UnpatchAll();
            CSM.Log("Destruction complete!");
        }

        private static void CreateJoinGameButton()
        {
            // Temp, we do not want to run this code on release as it's not ready yet.
            return;

            var uiView = UIView.GetAView().FindUIComponent("Menu") as UIPanel;

            if (uiView == null)
                return;

            var joinGameButton = UIView.GetAView().FindUIComponent("JoinGame") as UIButton;

            // Create the button if it does not exist
            if (joinGameButton == null)
                joinGameButton = (UIButton)uiView.AddUIComponent(typeof(UIButton));

            joinGameButton.name = "JoinGame";
            joinGameButton.text = "JOIN GAME";
            joinGameButton.width = 411;
            joinGameButton.height = 56;

            joinGameButton.textHorizontalAlignment = UIHorizontalAlignment.Center;

            joinGameButton.focusedColor = new Color32(254, 254, 254, 255);
            joinGameButton.focusedTextColor = new Color32(255, 255, 255, 255);

            joinGameButton.hoveredColor = new Color32(94, 195, 255, 255);
            joinGameButton.hoveredFgSprite = "MenuPanelInfo";
            joinGameButton.hoveredTextColor = new Color32(7, 123, 255, 255);

            joinGameButton.bottomColor = new Color32(163, 226, 254, 255);

            joinGameButton.textColor = new Color32(254, 254, 254, 255);
            joinGameButton.textScale = 1.5f;

            joinGameButton.pressedColor = new Color32(185, 221, 254, 255);
            joinGameButton.pressedFgSprite = "MenuPanelInfo";
            joinGameButton.pressedTextColor = new Color32(30, 30, 44, 255);

            joinGameButton.useDropShadow = true;
            joinGameButton.useGradient = true;
            joinGameButton.useGUILayout = true;

            joinGameButton.dropShadowOffset = new Vector2(0, -1.33f);

            // TODO, enable later
            //    joinGameButton.eventClick -= JoinGameButton_eventClick;
            //    joinGameButton.eventClick += JoinGameButton_eventClick;
        }

        private static void JoinGameButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            var panel = UIView.GetAView().FindUIComponent<JoinGamePanel>("MPJoinGamePanel");

            if (panel != null)
            {
                panel.isVisible = true;
                panel.Focus();
            }
            else
            {
                var joinGamePanel = (JoinGamePanel)UIView.GetAView().AddUIComponent(typeof(JoinGamePanel));
                joinGamePanel.Focus();
            }
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