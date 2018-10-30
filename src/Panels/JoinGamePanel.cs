using ColossalFramework;
using ColossalFramework.UI;
using CSM.Helpers;
using CSM.Networking;
using UnityEngine;

namespace CSM.Panels
{
    public class JoinGamePanel : UIPanel
    {
        private UITextField _ipAddressField;
        private UITextField _portField;
        private UITextField _nameField;
        private UITextField _passwordField;

        private UILabel _connectionStatus;

        private UIButton _connectButton;
        private UIButton _closeButton;

        public override void Start()
        {
            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            name = "MPJoinGamePanel";
            color = new Color32(110, 110, 110, 250);

            // Grab the view for calculating width and height of game
            var view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(view.fixedWidth / 2.0f - 180.0f, view.fixedHeight / 2.0f - 250.0f);

            width = 360;
            height = 560;

            // Title Label
            this.CreateTitleLabel("Connect to Server", new Vector2(80, -20));

            // IP Address Label
            this.CreateLabel("IP Address:", new Vector2(10, -70));

            // IP Address field
            _ipAddressField = this.CreateTextField("localhost", new Vector2(10, -100));

            // Port Label
            this.CreateLabel("Port:", new Vector2(10, -150));

            // Port field
            _portField = this.CreateTextField("4230", new Vector2(10, -180));
            _portField.numericalOnly = true;

            // Username label
            this.CreateLabel("Username:", new Vector2(10, -230));

            // Username field
            _nameField = this.CreateTextField("", new Vector2(10, -260));

            // Password label
            this.CreateLabel("Password:", new Vector2(10, -310));

            // Password field
            _passwordField = this.CreateTextField("", new Vector2(10, -340));
            _passwordField.isPasswordField = true;

            // Connect to Server Button
            _connectButton = this.CreateButton("Connect to Server", new Vector2(10, -420));
            _connectButton.eventClick += OnConnectButtonClick;

            // Close this dialog
            _closeButton = this.CreateButton("Cancel", new Vector2(10, -490));
            _closeButton.eventClick += (component, param) =>
            {
                isVisible = false;
            };

            _connectionStatus = this.CreateLabel("Not Connected", new Vector2(10, -395));
            _connectionStatus.textAlignment = UIHorizontalAlignment.Center;
            _connectionStatus.textColor = new Color32(255, 0, 0, 255);
        }

        private void OnConnectButtonClick(UIComponent uiComponent, UIMouseEventParameter eventParam)
        {
            _connectionStatus.textColor = new Color32(255, 255, 0, 255);
            _connectionStatus.text = "Connecting...";

            if (string.IsNullOrEmpty(_portField.text) || string.IsNullOrEmpty(_ipAddressField.text))
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Invalid Port or IP";
                return;
            }

            if (string.IsNullOrEmpty(_nameField.text))
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Invalid Username";
                return;
            }

            if (!int.TryParse(_portField.text, out var port))
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Invalid Port";
                return;
            }

            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Already Running Server";
                return;
            }

            // Try connect and get the result
            MultiplayerManager.Instance.ConnectToServer(_ipAddressField.text, port, _nameField.text, _passwordField.text, (success) => {
                Singleton<SimulationManager>.instance.m_ThreadingWrapper.QueueMainThread(() => {
                    if (!success)
                    {
                        _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                        _connectionStatus.text = MultiplayerManager.Instance.CurrentClient.ConnectionMessage;
                        CSM.Log(MultiplayerManager.Instance.CurrentClient.ConnectionMessage);
                    }
                    else
                    {
                        _connectionStatus.text = "";
                        CSM.Log("Connected!");

                        isVisible = false;
                    }
                });
            });
        }
    }
}