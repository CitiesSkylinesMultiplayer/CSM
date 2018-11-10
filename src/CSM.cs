using ColossalFramework.UI;
using CSM.Common;
using CSM.Panels;
using System.IO;
using UnityEngine;

namespace CSM
{
    public class CSM : ICities.IUserMod
    {
        public CSM()
        {
            // Make sure the button is enabled after intro load
            LoadingManager.instance.m_introLoaded += () => CreateJoinGameButton();

            // We are in the main menu, so load the "Join Game" button.
            if (!LoadingManager.instance.m_essentialScenesLoaded)
            {
                CreateJoinGameButton();
            }

            // Delete the log file on startup / reload
            File.Delete("multiplayer-log.txt");
        }

        private static void CreateJoinGameButton()
        {
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

        public string Name => "CSM";

        public string Description => "Multiplayer mod for Cities: Skylines.";

        /// <summary>
        ///     Log a message to the console.
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            LogManager.LogMessage(message);
        }
    }
}