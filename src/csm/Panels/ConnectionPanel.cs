using ColossalFramework.UI;
using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Status;
using UnityEngine;

namespace CSM.Panels
{
    public class ConnectionPanel : UIPanel
    {
        private UIButton _clientConnectButton;
        private UIButton _serverConnectButton;

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
                    if (MultiplayerManager.Instance.CurrentServer.Status == ServerStatus.Running)
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

            this.CreateTitleLabel("Multiplayer Menu", new Vector3(80, -20, 0));

            // Join game button
            _clientConnectButton = this.CreateButton("Join Game", new Vector2(10, -60));

            // Manage server button
            _serverManageButton = this.CreateButton("Manage Server", new Vector2(10, -60));
            _serverManageButton.isEnabled = false;
            _serverManageButton.isVisible = false;

            // Host game button
            _serverConnectButton = this.CreateButton("Host Game", new Vector2(10, -130));

            // Close server button
            _serverDisconnectButton = this.CreateButton("Close Server", new Vector2(10, -130));
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
