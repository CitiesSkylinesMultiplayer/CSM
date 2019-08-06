using ColossalFramework.UI;
using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Status;
using UnityEngine;
using CSM.Localisation;

namespace CSM.Panels
{
    public class ConnectionPanel : UIPanel
    {
        private UIButton _clientConnectButton;
        private UIButton _serverConnectButton;

        // These buttons are displayed when the server is running
        private UIButton _disconnectButton;

        private UIButton _serverManageButton;

        private UIButton _serverPlayerButton;

        public override void Start()
        {
            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            name = "MPConnectionPanel";
            color = new Color32(110, 110, 110, 250);

            ChatLogPanel.ActiveWindows.Add(name);

            // Grab the view for calculating width and height of game
            UIView view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(view.fixedWidth / 2.0f - 180.0f, view.fixedHeight / 2.0f - 100.0f);

            width = 360;
            height = 200;

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
                        Show(_serverPlayerButton);

                        _disconnectButton.text = Translation.PullTranslation("Disconnect");
                        _disconnectButton.position = new Vector2(10, -200);

                        height = 270;

                    }
                    else
                    {
                        Show(_clientConnectButton);
                        Show(_serverConnectButton);
                        Hide(_disconnectButton);
                        Hide(_serverManageButton);
                        Hide(_serverPlayerButton);

                        height = 200;
                    }
                }
                else if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
                {
                    Hide(_clientConnectButton);
                    Hide(_serverConnectButton);
                    Show(_disconnectButton);
                    Hide(_serverManageButton);

                    _disconnectButton.text = Translation.PullTranslation("Disconnect");
                }
                else
                {
                    Show(_clientConnectButton);
                    Show(_serverConnectButton);
                    Hide(_disconnectButton);
                    Hide(_serverManageButton);
                }
            };

            this.CreateTitleLabel(Translation.PullTranslation("MultiplayerMenu"), new Vector3(80, -20, 0));

            // Join game button
            _clientConnectButton = this.CreateButton(Translation.PullTranslation("JoinGame"), new Vector2(10, -60));
            _clientConnectButton.name = "MPClientConnectButton";

            // Manage server button
            _serverManageButton = this.CreateButton(Translation.PullTranslation("ManageServer"), new Vector2(10, -60));
            _serverManageButton.isEnabled = false;
            _serverManageButton.isVisible = false;
            _serverManageButton.name = "MPServerManageButton";

            // Manage player button
            _serverPlayerButton = this.CreateButton(Translation.PullTranslation("ManagePlayers"), new Vector2(10, -130));
            _serverPlayerButton.isEnabled = false;
            _serverPlayerButton.isVisible = false;
            _serverPlayerButton.name = "MPServerPlayerButton";

            // Host game button
            _serverConnectButton = this.CreateButton(Translation.PullTranslation("HostGame"), new Vector2(10, -130));
            _serverConnectButton.name = "MPServerConnectButton";

            // Close server button
            _disconnectButton = this.CreateButton(Translation.PullTranslation("StopServer"), new Vector2(10, -130));
            _disconnectButton.isEnabled = false;
            _disconnectButton.isVisible = false;
            _disconnectButton.name = "MPDisconnectButton";

            _clientConnectButton.eventClick += (component, param) =>
            {
                JoinGamePanel panel = view.FindUIComponent<JoinGamePanel>("MPJoinGamePanel");
                //ServerListPanel panel = view.FindUIComponent<ServerListPanel>("MPServerListPanel");

                if (panel != null)
                {
                    panel.isVisible = true;
                    panel.Focus();
                }
                else
                {
                    //ServerListPanel ServerListPanel = (ServerListPanel)view.AddUIComponent(typeof(ServerListPanel));
                    JoinGamePanel JoinGamePanel = (JoinGamePanel)view.AddUIComponent(typeof(JoinGamePanel));
                    JoinGamePanel.Focus();
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

            _serverPlayerButton.eventClick += (component, param) =>
            {
                /*
                PlayerListPanel panel = view.FindUIComponent<PlayerListPanel>("MPPlayerListPanel");

                if (panel != null)
                {
                    panel.isVisible = true;
                }
                else
                {
                    panel = (PlayerListPanel)view.AddUIComponent(typeof(PlayerListPanel));
                }

                panel.Focus();

                isVisible = false;
                */
            };
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