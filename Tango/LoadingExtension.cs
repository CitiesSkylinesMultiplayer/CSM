using ColossalFramework.UI;
using ICities;
using Tango.Networking;
using Tango.Panels;
using UnityEngine;

namespace Tango
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private UIButton _muiltiplayerButton;

        public override void OnLevelUnloading()
        {
            // Stop server (only if running, checks done in method)
            MultiplayerManager.Instance.StopGameServer();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            var uiView = UIView.GetAView();

            _muiltiplayerButton = (UIButton)uiView.AddUIComponent(typeof(UIButton));

            _muiltiplayerButton.text = "Show Muiltiplayer Menu";
            _muiltiplayerButton.width = 240;
            _muiltiplayerButton.height = 40;

            _muiltiplayerButton.normalBgSprite = "ButtonMenu";
            _muiltiplayerButton.disabledBgSprite = "ButtonMenuDisabled";
            _muiltiplayerButton.hoveredBgSprite = "ButtonMenuHovered";
            _muiltiplayerButton.focusedBgSprite = "ButtonMenuFocused";
            _muiltiplayerButton.pressedBgSprite = "ButtonMenuPressed";
            _muiltiplayerButton.textColor = new Color32(255, 255, 255, 255);
            _muiltiplayerButton.disabledTextColor = new Color32(7, 7, 7, 255);
            _muiltiplayerButton.hoveredTextColor = new Color32(7, 132, 255, 255);
            _muiltiplayerButton.focusedTextColor = new Color32(255, 255, 255, 255);
            _muiltiplayerButton.pressedTextColor = new Color32(30, 30, 44, 255);

            // Enable button sounds.
            _muiltiplayerButton.playAudioEvents = true;

            // Place the button.
            _muiltiplayerButton.transformPosition = new Vector3(-1.65f, 0.97f);

            // Respond to button click.
            _muiltiplayerButton.eventClick += (component, param) =>
            {
                var panel = uiView.FindUIComponent<ConnectionPanel>("MPConnectionPanel");

                if (panel != null)
                {
                    if (panel.isVisible)
                    {
                        panel.isVisible = false;
                        _muiltiplayerButton.text = "Show Muiltiplayer Menu";
                    }
                    else
                    {
                        panel.isVisible = true;
                        _muiltiplayerButton.text = "Hide Muiltiplayer Menu";
                    }
                }
                else
                {
                    var newConnectionPanel = (ConnectionPanel)uiView.AddUIComponent(typeof(ConnectionPanel));
                    _muiltiplayerButton.text = "Hide Muiltiplayer Menu";

                    // Bind visibility changed event to update button text
                    newConnectionPanel.eventVisibilityChanged +=
                        (uiComponent, value) =>
                        {
                            _muiltiplayerButton.text = uiComponent.isVisible ? "Hide Muiltiplayer Menu" : "Show Muiltiplayer Menu";
                        };
                }
            };
        }
    }
}
