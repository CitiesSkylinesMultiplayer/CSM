using ColossalFramework.UI;
using CSM.Panels;
using HarmonyLib;
using NLog;
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
            MainMenuHandler.CreateOrUpdateJoinGameButton();
        }
    }

    public static class MainMenuHandler
    {
        private static readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        public static void CreateOrUpdateJoinGameButton()
        {
            _logger.Info("Creating join game button...");

            UIPanel uiView = UIView.GetAView().FindUIComponent("Menu") as UIPanel;

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
                    JoinGamePanel panel = UIView.GetAView().FindUIComponent<JoinGamePanel>("MPJoinGamePanel");

                    if (panel != null)
                    {
                        panel.isVisible = true;
                        panel.Focus();
                    }
                    else
                    {
                        JoinGamePanel joinGamePanel = (JoinGamePanel)UIView.GetAView().AddUIComponent(typeof(JoinGamePanel));
                        joinGamePanel.RequestWorld = true;
                        joinGamePanel.Focus();
                    }
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
