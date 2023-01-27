using System;
using System.Reflection;
using System.Threading;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using CSM.API;
using CSM.Panels;
using CSM.Util;
using HarmonyLib;
using UnityEngine;

namespace CSM.Injections
{
    [HarmonyPatch(typeof(MainMenu))]
    [HarmonyPatch("Awake")]
    public class MainMenuAwake
    {
        /// <summary>
        ///     This handler is used to place the 'JOIN GAME' button to the start
        ///     of the main menu (handles reloading the intro etc.
        /// </summary>
        public static void Prefix()
        {
            MainMenuHandler.Init();
        }
    }

    public static class MainMenuHandler
    {
        public static void Init()
        {
            CreateOrUpdateJoinGameButton();
            new Thread(() => CheckForUpdate(false)).Start();
        }

        public static void CheckForUpdate(bool alwaysShowInfo)
        {
            try
            {
                string latest = new CSMWebClient().DownloadString($"http://{CSM.Settings.ApiServer}/api/version");
                latest = latest.Substring(1);
                string[] versionParts = latest.Split('.');
                Version latestVersion = new Version(int.Parse(versionParts[0]), int.Parse(versionParts[1]));

                Version version = Assembly.GetAssembly(typeof(CSM)).GetName().Version;
                if (latestVersion > version)
                {
                    Log.Info(
                        $"Update available! Current version: {version.Major}.{version.Minor} Latest version: {latestVersion.Major}.{latestVersion.Minor}");
                    ThreadHelper.dispatcher.Dispatch(() =>
                    {
                        MessagePanel panel = PanelManager.ShowPanel<MessagePanel>();
                        panel.DisplayUpdateAvailable(version, latestVersion);
                    });
                }
                else if (alwaysShowInfo)
                {
                    ThreadHelper.dispatcher.Dispatch(() =>
                    {
                        MessagePanel panel = PanelManager.ShowPanel<MessagePanel>();
                        panel.DisplayNoUpdateAvailable();
                    });
                }
            }
            catch (Exception e)
            {
                Log.Warn($"Failed to check for updates: {e.Message}");
            }
        }

        private static void CreateOrUpdateJoinGameButton()
        {
            Log.Info("Creating join game button...");

            UIPanel uiView = UIView.GetAView()?.FindUIComponent("Menu") as UIPanel;

            if (uiView == null)
                return;

            UIButton joinGameButton = UIView.GetAView().FindUIComponent("JoinGame") as UIButton;

            // Create the button if it does not exist and assign
            // the click event.
            if (joinGameButton == null)
            {
                joinGameButton = (UIButton)uiView.AddUIComponent(typeof(UIButton));
                joinGameButton.eventClick += (s, e) =>
                {
                    PanelManager.ShowPanel<JoinGamePanel>();
                };
            }

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
        }
    }
}
