using ColossalFramework.UI;
using Tango.Networking;
using UnityEngine;

namespace Tango.Panels
{
    public class ConnectionPanel : UIPanel
    {
        private UIButton _clientConnectButton;
        private UIButton _serverConnectButton;
        private UILabel _title;

        // These buttons are displayed when the server is running
        private UIButton _serverDisconnectButton;
        private UIButton _serverManageButton;

        public override void Start()
        {
            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            name = "MPConnectionPanel";
            color = new Color32(110, 110, 110, 250);

            // Grab the view for caculating width and height of game
            var view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(view.fixedWidth / 2.0f - 180.0f, view.fixedHeight / 2.0f - 100.0f);

            width = 360;
            height = 200;

            // Handle visible change events
            eventVisibilityChanged += (component, value) =>
            {
                if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
                {
                    if (MultiplayerManager.Instance.CurrentServer.IsServerStarted)
                    {
                        _clientConnectButton.isEnabled = false;
                        _clientConnectButton.isVisible = false;

                        _serverConnectButton.isEnabled = false;
                        _serverConnectButton.isVisible = false;

                        _serverDisconnectButton.isEnabled = true;
                        _serverDisconnectButton.isVisible = true;

                        _serverManageButton.isEnabled = true;
                        _serverManageButton.isVisible = true;
                    }
                    else
                    {
                        _clientConnectButton.isEnabled = true;
                        _clientConnectButton.isVisible = true;

                        _serverConnectButton.isEnabled = true;
                        _serverConnectButton.isVisible = true;

                        _serverDisconnectButton.isEnabled = false;
                        _serverDisconnectButton.isVisible = false;

                        _serverManageButton.isEnabled = false;
                        _serverManageButton.isVisible = false;
                    }
                }
                else if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
                {
                    _clientConnectButton.isEnabled = true;
                    _clientConnectButton.isVisible = true;

                    _serverConnectButton.isEnabled = true;
                    _serverConnectButton.isVisible = true;

                    _serverDisconnectButton.isEnabled = false;
                    _serverDisconnectButton.isVisible = false;

                    _serverManageButton.isEnabled = false;
                    _serverManageButton.isVisible = false;
                }
                else
                {
                    _clientConnectButton.isEnabled = true;
                    _clientConnectButton.isVisible = true;

                    _serverConnectButton.isEnabled = true;
                    _serverConnectButton.isVisible = true;

                    _serverDisconnectButton.isEnabled = false;
                    _serverDisconnectButton.isVisible = false;

                    _serverManageButton.isEnabled = false;
                    _serverManageButton.isVisible = false;
                }                 
            };

            _title = (UILabel)AddUIComponent(typeof(UILabel));
            _title.position = new Vector3(0,0,0);
            _title.text = "Multiplayer Menu";
            _title.textAlignment = UIHorizontalAlignment.Center;
            _title.width = 340;
            _title.textScale = 1.2f;
            _title.opacity = 0.8f;
            _title.height = 60;
            _title.position = new Vector3(80, -20, 0);

            _clientConnectButton = (UIButton) AddUIComponent(typeof(UIButton));
            _clientConnectButton.position = new Vector3(10, -60, 0);
            _clientConnectButton.width = 340;
            _clientConnectButton.height = 60;
            _clientConnectButton.text = "Join Game";
            _clientConnectButton.normalBgSprite = "ButtonMenu";
            _clientConnectButton.disabledBgSprite = "ButtonMenuDisabled";
            _clientConnectButton.hoveredBgSprite = "ButtonMenuHovered";
            _clientConnectButton.focusedBgSprite = "ButtonMenu";
            _clientConnectButton.pressedBgSprite = "ButtonMenuPressed";
            _clientConnectButton.textColor = new Color32(255, 51, 153, 150);
            _clientConnectButton.disabledTextColor = new Color32(7, 7, 7, 200);
            _clientConnectButton.hoveredTextColor = new Color32(255, 255, 255, 255);
            _clientConnectButton.pressedTextColor = new Color32(204, 0, 0, 255);
            _clientConnectButton.playAudioEvents = true;

            _serverManageButton = (UIButton)AddUIComponent(typeof(UIButton));
            _serverManageButton.position = new Vector3(10, -60, 0);
            _serverManageButton.width = 340;
            _serverManageButton.height = 60;
            _serverManageButton.text = "Manage Server";
            _serverManageButton.normalBgSprite = "ButtonMenu";
            _serverManageButton.disabledBgSprite = "ButtonMenuDisabled";
            _serverManageButton.hoveredBgSprite = "ButtonMenuHovered";
            _serverManageButton.focusedBgSprite = "ButtonMenu";
            _serverManageButton.pressedBgSprite = "ButtonMenuPressed";
            _serverManageButton.textColor = new Color32(255, 51, 153, 150);
            _serverManageButton.disabledTextColor = new Color32(7, 7, 7, 200);
            _serverManageButton.hoveredTextColor = new Color32(255, 255, 255, 255);
            _serverManageButton.pressedTextColor = new Color32(204, 0, 0, 255);
            _serverManageButton.playAudioEvents = true;
            _serverManageButton.isEnabled = false;
            _serverManageButton.isVisible = false;

            _serverConnectButton = (UIButton)AddUIComponent(typeof(UIButton));
            _serverConnectButton.width = 340;
            _serverConnectButton.height = 60;
            _serverConnectButton.position = new Vector3(10, -130, 0);
            _serverConnectButton.text = "Host Game";
            _serverConnectButton.normalBgSprite = "ButtonMenu";
            _serverConnectButton.disabledBgSprite = "ButtonMenuDisabled";
            _serverConnectButton.hoveredBgSprite = "ButtonMenuHovered";
            _serverConnectButton.focusedBgSprite = "ButtonMenu";
            _serverConnectButton.pressedBgSprite = "ButtonMenuPressed";
            _serverConnectButton.textColor = new Color32(255, 51, 153, 150);
            _serverConnectButton.disabledTextColor = new Color32(7, 7, 7, 200);
            _serverConnectButton.hoveredTextColor = new Color32(255, 255, 255, 255);
            _serverConnectButton.pressedTextColor = new Color32(204, 0, 0, 255);
            _serverConnectButton.playAudioEvents = true;

            _serverDisconnectButton = (UIButton)AddUIComponent(typeof(UIButton));
            _serverDisconnectButton.width = 340;
            _serverDisconnectButton.height = 60;
            _serverDisconnectButton.position = new Vector3(10, -130, 0);
            _serverDisconnectButton.text = "Close Server";
            _serverDisconnectButton.normalBgSprite = "ButtonMenu";
            _serverDisconnectButton.disabledBgSprite = "ButtonMenuDisabled";
            _serverDisconnectButton.hoveredBgSprite = "ButtonMenuHovered";
            _serverDisconnectButton.focusedBgSprite = "ButtonMenu";
            _serverDisconnectButton.pressedBgSprite = "ButtonMenuPressed";
            _serverDisconnectButton.textColor = new Color32(255, 51, 153, 150);
            _serverDisconnectButton.disabledTextColor = new Color32(7, 7, 7, 200);
            _serverDisconnectButton.hoveredTextColor = new Color32(255, 255, 255, 255);
            _serverDisconnectButton.pressedTextColor = new Color32(204, 0, 0, 255);
            _serverDisconnectButton.playAudioEvents = true;
            _serverDisconnectButton.isEnabled = false;
            _serverDisconnectButton.isVisible = false;

            _clientConnectButton.eventClick += (component, param) =>
            {
                var panel = view.FindUIComponent<JoinGamePanel>("MPJoinGamePanel");

                if (panel != null)
                {
                    panel.isVisible = true;
                    panel.Focus();
                }
                else
                {
                    var joinGamePanel = (JoinGamePanel)view.AddUIComponent(typeof(JoinGamePanel));
                    joinGamePanel.Focus();
                }

                isVisible = false;
            };

            _serverConnectButton.eventClick += (component, param) =>
            {
                MultiplayerManager.Instance.StartGameServer();

                isVisible = false;
            };

            _serverDisconnectButton.eventClick += (component, param) =>
            {
                isVisible = false;

                MultiplayerManager.Instance.StopGameServer();
            };

            _serverManageButton.eventClick += (component, param) =>
            {
                isVisible = false;
            };

            base.Start();
        }
    }
}
