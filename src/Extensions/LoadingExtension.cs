using ColossalFramework.UI;
using ColossalFramework.Globalization;
using CSM.Networking;
using CSM.Panels;
using ICities;
using UnityEngine;
using CSM.Localisation;

namespace CSM.Extensions
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private UIButton _multiplayerButton;

        public override void OnReleased()
        {
            // Stop everything
            MultiplayerManager.Instance.StopEverything();

            base.OnReleased();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            var uiView = UIView.GetAView();

            // Add the chat log
            uiView.AddUIComponent(typeof(ChatLogPanel));

            _multiplayerButton = (UIButton)uiView.AddUIComponent(typeof(UIButton));

            _multiplayerButton.text = Translation.PullTranslation("Multiplayer");
            _multiplayerButton.width = 150;
            _multiplayerButton.height = 40;

            _multiplayerButton.normalBgSprite = "ButtonMenu";
            _multiplayerButton.disabledBgSprite = "ButtonMenuDisabled";
            _multiplayerButton.hoveredBgSprite = "ButtonMenuHovered";
            _multiplayerButton.focusedBgSprite = "ButtonMenuFocused";
            _multiplayerButton.pressedBgSprite = "ButtonMenuPressed";
            _multiplayerButton.textColor = new Color32(255, 255, 255, 255);
            _multiplayerButton.disabledTextColor = new Color32(7, 7, 7, 255);
            _multiplayerButton.hoveredTextColor = new Color32(7, 132, 255, 255);
            _multiplayerButton.focusedTextColor = new Color32(255, 255, 255, 255);
            _multiplayerButton.pressedTextColor = new Color32(30, 30, 44, 255);

            // Enable button sounds.
            _multiplayerButton.playAudioEvents = true;

            // Place the button.
            _multiplayerButton.relativePosition = new Vector3(uiView.fixedWidth / 2.0f - 780.0f, 12.0f);

            // Respond to button click.
            _multiplayerButton.eventClick += (component, param) =>
            {
                if (Translation.TranslationName != LocaleManager.cultureInfo.Name.Substring(0, 2))
                {
                    Translation.GetXMLTranslation();
                    _multiplayerButton.text = Translation.PullTranslation("Multiplayer");

                    ChatLogPanel.DestroyAllUIComponents();

                    ConnectionPanel newConnectionPanel = (ConnectionPanel)uiView.AddUIComponent(typeof(ConnectionPanel));

                    ChatLogPanel newChatLogPanel = (ChatLogPanel)uiView.AddUIComponent(typeof(ChatLogPanel));
                }

                else
                {
                    ConnectionPanel panel = uiView.FindUIComponent<ConnectionPanel>("MPConnectionPanel");

                    if (panel != null)
                    {
                        panel.isVisible = !panel.isVisible;
                        _multiplayerButton.Unfocus();
                    }
                    else
                    {
                        ConnectionPanel newConnectionPanel = (ConnectionPanel)uiView.AddUIComponent(typeof(ConnectionPanel));
                    }
                }
            };
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            //Code below destroys any created UI from the screen.
            UIComponent _getui = UIView.GetAView().FindUIComponent<UIComponent>("ChatLogPanel");

            UIComponent[] children = _getui.GetComponentsInChildren<UIComponent>();

            foreach (UIComponent child in children)
            {
                UIView.Destroy(child);
            }

            // Destroy duplicated multiplayer button
            UIComponent temp = UIView.GetAView().FindUIComponent("MPConnectionPanel");
            UIView.Destroy(temp);
        }
    }
}