using ColossalFramework.UI;
using Tango.Networking;
using UnityEngine;

namespace Tango.Panels
{
    public class ConnectionPanel : UIPanel
    {
        private UIButton _clientConnectButton;
        private UIButton _serverConnectButton;

        public override void Start()
        {
            base.Start();

            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            name = "MPConnectionPanel";
            color = new Color32(20, 20, 20, 220);

            // Handle visible change events
            eventVisibilityChanged += (component, value) =>
            {
                if (Server.Instance.IsServerStarted)
                {
                    _clientConnectButton.isEnabled = false;
                    _serverConnectButton.isEnabled = false;
                }
                else
                {
                    _clientConnectButton.isEnabled = true;
                    _serverConnectButton.isEnabled = true;
                }
            };

            _clientConnectButton = (UIButton) AddUIComponent(typeof(UIButton));
            _clientConnectButton.position = new Vector3(40, -240, 0);
            _clientConnectButton.text = "Connect to Server";
            _clientConnectButton.normalBgSprite = "ButtonMenu";
            _clientConnectButton.disabledBgSprite = "ButtonMenuDisabled";
            _clientConnectButton.hoveredBgSprite = "ButtonMenuHovered";
            _clientConnectButton.focusedBgSprite = "ButtonMenuFocused";
            _clientConnectButton.pressedBgSprite = "ButtonMenuPressed";
            _clientConnectButton.textColor = new Color32(255, 51, 153, 150);
            _clientConnectButton.disabledTextColor = new Color32(7, 7, 7, 200);
            _clientConnectButton.hoveredTextColor = new Color32(255, 255, 255, 255);
            _clientConnectButton.pressedTextColor = new Color32(204, 0, 0, 255);
            _clientConnectButton.playAudioEvents = true;

            _serverConnectButton = (UIButton)AddUIComponent(typeof(UIButton));
            _serverConnectButton.position = new Vector3(40, -10, 0);
            _serverConnectButton.text = "Host Server";
            _serverConnectButton.normalBgSprite = "ButtonMenu";
            _serverConnectButton.disabledBgSprite = "ButtonMenuDisabled";
            _serverConnectButton.hoveredBgSprite = "ButtonMenuHovered";
            _serverConnectButton.focusedBgSprite = "ButtonMenuFocused";
            _serverConnectButton.pressedBgSprite = "ButtonMenuPressed";
            _serverConnectButton.textColor = new Color32(255, 51, 153, 150);
            _serverConnectButton.disabledTextColor = new Color32(7, 7, 7, 200);
            _serverConnectButton.hoveredTextColor = new Color32(255, 255, 255, 255);
            _serverConnectButton.pressedTextColor = new Color32(204, 0, 0, 255);
            _serverConnectButton.playAudioEvents = true;

            _clientConnectButton.eventClick += (component, param) =>
            {

                isVisible = false;
            };

            _serverConnectButton.eventClick += (component, param) =>
            {
                Server.Instance.StartServer();

                isVisible = false;
            };
        }
    }
}
