using ColossalFramework.UI;
using CSM.Panels;
using UnityEngine;

namespace CSM
{
    public class CSM : ICities.IUserMod
    {
        public CSM()
        {
            var uiView = UIView.GetAView().FindUIComponent("Menu") as UIPanel;

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

            joinGameButton.eventClick -= JoinGameButton_eventClick;
            joinGameButton.eventClick += JoinGameButton_eventClick;
        }

        private void JoinGameButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            CSM.Log("test");
            LoadingManager.instance.LoadIntro();
        }

        public string Name => "CSM";

        public string Description => "Multiplayer mod for Cities: Skylines.";

        /// <summary>
        ///     Log a message to the console.
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            // Console
            Debug.Log($"[CSM] {message}");

            // Game console
            var chatLog = UIView.GetAView().FindUIComponent<ChatLogPanel>("MPChatLogPanel");
            chatLog?.AddMessage($"[CSM] {message}");
        }
    }
}