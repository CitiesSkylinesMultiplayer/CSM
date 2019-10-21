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
        private UIButton _disconnectButton;

        private UIButton _serverManageButton;

        private UICheckBox _playerPointers;
        public static bool showPlayerPointers = false;

        public override void Start()
        {
            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            name = "MPConnectionPanel";
            color = new Color32(110, 110, 110, 250);

            // Grab the view for calculating width and height of game
            UIView view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(view.fixedWidth / 2.0f - 180.0f, view.fixedHeight / 2.0f - 100.0f);

            width = 360;
            height = 240;

            // Handle visible change events
            eventVisibilityChanged += (component, visible) =>
            {
                if (!visible)
                    return;

                if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
                {
                    if (MultiplayerManager.Instance.CurrentServer.Status == ServerStatus.Running)
                    {
                        Hide(_clientConnectButton);
                        Hide(_serverConnectButton);
                        Show(_disconnectButton);
                        Show(_serverManageButton);
                        _playerPointers.isVisible = true;
                        _playerPointers.isEnabled = true;

                        _disconnectButton.text = "Stop server";
                    }
                    else
                    {
                        Show(_clientConnectButton);
                        Show(_serverConnectButton);
                        Hide(_disconnectButton);
                        Hide(_serverManageButton);

                        _playerPointers.isVisible = false;
                        _playerPointers.isEnabled = false;
                    }
                }
                else if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
                {
                    Hide(_clientConnectButton);
                    Hide(_serverConnectButton);
                    Show(_disconnectButton);
                    Hide(_serverManageButton);

                    _playerPointers.isVisible = true;
                    _playerPointers.isEnabled = true;

                    _disconnectButton.text = "Disconnect";
                }
                else
                {
                    Show(_clientConnectButton);
                    Show(_serverConnectButton);
                    Hide(_disconnectButton);
                    Hide(_serverManageButton);

                    _playerPointers.isVisible = false;
                    _playerPointers.isEnabled = false;
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
            _disconnectButton = this.CreateButton("Stop Server", new Vector2(10, -130));
            _disconnectButton.isEnabled = false;
            _disconnectButton.isVisible = false;

            // Show Player Pointers
            _playerPointers = this.CreateCheckBox("Show Player Pointers", new Vector2(10, -210));
            _playerPointers.isVisible = false;
            _playerPointers.isEnabled = false;
            _playerPointers.eventClicked += (component, param) =>
            {
                showPlayerPointers = _playerPointers.isChecked;
            };

            _clientConnectButton.eventClick += (component, param) =>
            {
                JoinGamePanel panel = view.FindUIComponent<JoinGamePanel>("MPJoinGamePanel");

                if (panel != null)
                {
                    panel.isVisible = true;
                    panel.Focus();
                }
                else
                {
                    JoinGamePanel joinGamePanel = (JoinGamePanel)view.AddUIComponent(typeof(JoinGamePanel));
                    joinGamePanel.Focus();
                }

                isVisible = false;
            };

            // Host a game panel
            _serverConnectButton.eventClick += (component, param) =>
            {
                HostGamePanel panel = view.FindUIComponent<HostGamePanel>("MPHostGamePanel");

                if (panel != null)
                {
                    panel.isVisible = true;
                    panel.Focus();
                }
                else
                {
                    HostGamePanel hostGamePanel = (HostGamePanel)view.AddUIComponent(typeof(HostGamePanel));
                    hostGamePanel.Focus();
                }

                isVisible = false;
            };

            _disconnectButton.eventClick += (component, param) =>
            {
                isVisible = false;

                MultiplayerManager.Instance.StopEverything();
            };

            _serverManageButton.eventClick += (component, param) =>
            {
                ManageGamePanel panel = view.FindUIComponent<ManageGamePanel>("MPManageGamePanel");

                if (panel != null)
                {
                    panel.isVisible = true;
                }
                else
                {
                    panel = (ManageGamePanel)view.AddUIComponent(typeof(ManageGamePanel));
                }

                panel.Focus();

                isVisible = false;
            };

            base.Start();
        }

        private void Show(UIButton button)
        {
            button.isVisible = true;
            button.isEnabled = true;
        }

        private void Hide(UIButton button)
        {
            button.isVisible = false;
            button.isEnabled = false;
        }
    }
}
