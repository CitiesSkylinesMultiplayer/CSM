using System;
using ColossalFramework.PlatformServices;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using CSM.API.Commands;
using CSM.Helpers;
using CSM.Mods;
using CSM.Networking;
using CSM.Networking.Config;
using UnityEngine;

namespace CSM.Panels
{
    public class JoinGamePanel : UIPanel
    {
        private UITextField _ipAddressField;
        private UITextField _portField;
        private UITextField _usernameField;
        private UITextField _passwordField;

        private UILabel _connectionStatus;

        private UIButton _connectButton;
        private UIButton _closeButton;
        private UIButton _troubleshootingButton;

        private UICheckBox _passwordBox;
        private UICheckBox _rememberBox;

        public static Color playerColor = Color.red;

        private ClientConfig _clientConfig;
        private bool _hasRemembered;

        private Action _onStarted = null;
        private bool _autofilledFields = false;

        public override void Start()
        {
            _hasRemembered = ConfigData.Load(ref _clientConfig, ConfigData.ClientFile);

            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            color = new Color32(110, 110, 110, 255);

            width = 360;
            height = 585;
            relativePosition = PanelManager.GetCenterPosition(this);

            // Title Label
            this.CreateTitleLabel("Connect to Server", new Vector2(80, -20));

            // IP Address Label
            this.CreateLabel("IP Address:", new Vector2(10, -70));

            // IP Address field
            _ipAddressField = this.CreateTextField(_clientConfig.HostAddress, new Vector2(10, -100));

            // Port Label
            this.CreateLabel("Port:", new Vector2(10, -150));

            // Port field
            _portField = this.CreateTextField(_clientConfig.Port.ToString(), new Vector2(10, -180));
            _portField.numericalOnly = true;

            // Username label
            this.CreateLabel("Username:", new Vector2(10, -230));

            // Username field
            _usernameField = this.CreateTextField(_clientConfig.Username, new Vector2(10, -260));
            if (PlatformService.active && PlatformService.personaName != null && _clientConfig.Username == "")
            {
                _usernameField.text = PlatformService.personaName;
            }

            // Password label
            this.CreateLabel("Password:", new Vector2(10, -310));

            // Password checkbox
            _passwordBox = this.CreateCheckBox("Show Password", new Vector2(120, -310));

            _passwordBox.eventClicked += (component, param) =>
            {
                _passwordField.isPasswordField = !_passwordBox.isChecked;
            };

            // Password field
            _passwordField = this.CreateTextField(_clientConfig.Password, new Vector2(10, -340));
            _passwordField.isPasswordField = true;

            // Remember-Me checkbox
            _rememberBox = this.CreateCheckBox("Remember Me", new Vector2(10, -390));
            _rememberBox.isChecked = _hasRemembered;

            // Connect to Server Button
            _connectButton = this.CreateButton("Connect to Server", new Vector2(10, -445));
            _connectButton.eventClick += OnConnectButtonClick;

            // Close this dialog
            _closeButton = this.CreateButton("Cancel", new Vector2(10, -515));
            _closeButton.eventClick += (component, param) =>
            {
                isVisible = false;
                MultiplayerManager.Instance.CurrentClient.StopMainMenuEventProcessor();
            };

            _connectionStatus = this.CreateLabel("", new Vector2(10, -420));
            _connectionStatus.textAlignment = UIHorizontalAlignment.Center;
            _connectionStatus.textColor = new Color32(255, 0, 0, 255);

            _troubleshootingButton = this.CreateButton("?", new Vector2(170, -420), 25, 20);
            _troubleshootingButton.isVisible = false;
            _troubleshootingButton.eventClick += (component, param) =>
            {
                MessagePanel panel = PanelManager.ShowPanel<MessagePanel>();
                int.TryParse(_portField.text, out int port);
                panel.DisplayTroubleshooting(false, port);
            };

            ModCompat.BuildModInfo(this);

            eventVisibilityChanged += (comp, visible) =>
            {
                if (visible)
                {
                    ModCompat.BuildModInfo(this);

                    if (_autofilledFields)
                    {
                        _ipAddressField.text = _clientConfig.HostAddress;
                        _portField.text = _clientConfig.Port.ToString();
                        _usernameField.text = _clientConfig.Username;
                        _passwordField.text = _clientConfig.Password;
                        _rememberBox.isChecked = _hasRemembered;
                        _connectionStatus.text = "";
                    }
                }
            };

            _onStarted?.Invoke();
        }

        private void OnConnectButtonClick(UIComponent uiComponent, UIMouseEventParameter eventParam)
        {
            string ipOrToken = _ipAddressField.text;
            if (ipOrToken.StartsWith("token:"))
            {
                string token = ipOrToken.Split(':')[1];
                _clientConfig = new ClientConfig(token, _usernameField.text, _passwordField.text);
            }
            else
            {
                _clientConfig = new ClientConfig(_ipAddressField.text, int.Parse(_portField.text), _usernameField.text,
                    _passwordField.text);

                if (!_autofilledFields || _rememberBox.isChecked)
                {
                    ConfigData.Save(ref _clientConfig, ConfigData.ClientFile, _rememberBox.isChecked);
                }
            }

            MultiplayerManager.Instance.CurrentClient.StartMainMenuEventProcessor();

            _connectionStatus.textColor = new Color32(255, 255, 0, 255);
            _connectionStatus.text = "Connecting...";
            _troubleshootingButton.isVisible = false;

            if ((!_clientConfig.TokenBased && string.IsNullOrEmpty(_portField.text)) || string.IsNullOrEmpty(_ipAddressField.text))
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Invalid Port or IP";
                return;
            }

            if (string.IsNullOrEmpty(_usernameField.text))
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Invalid Username";
                return;
            }

            if (!int.TryParse(_portField.text, out int port) && !_clientConfig.TokenBased)
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

            MultiplayerManager.Instance.ConnectToServer(_clientConfig, (success) =>
            {
                ThreadHelper.dispatcher.Dispatch(() =>
                {
                    if (!success)
                    {
                        _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                        _connectionStatus.text = MultiplayerManager.Instance.CurrentClient.ConnectionMessage;
                        if (MultiplayerManager.Instance.CurrentClient.ShowTroubleshooting)
                        {
                            _troubleshootingButton.isVisible = true;
                        }
                    }
                    else
                    {
                        // See WorldTransferHandler for actual loading
                        _connectionStatus.text = "";
                        isVisible = false;

                        MultiplayerManager.Instance.BlockGameFirstJoin();
                    }
                });
            });
        }

        public void FillFieldsOnError(string token, string username, string errorMsg)
        {
            void Action()
            {
                _autofilledFields = true;

                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = errorMsg;

                _ipAddressField.text = $"token:{token}";
                _portField.text = "<auto>";
                _usernameField.text = username;
                _passwordField.text = "";
                _rememberBox.isChecked = false;
            }

            if (_connectionStatus != null)
            {
                Action();
            }
            else
            {
                _onStarted = Action;
            }
        }

        public void SetConnecting()
        {
            void Action()
            {
                _autofilledFields = true;

                _connectionStatus.textColor = new Color32(255, 255, 0, 255);
                _connectionStatus.text = "Connecting...";
                _troubleshootingButton.isVisible = false;

                _ipAddressField.text = "...";
                _portField.text = "...";
                _usernameField.text = "...";
                _passwordField.text = "...";
                _rememberBox.isChecked = false;
            }

            if (_connectionStatus != null)
            {
                Action();
            }
            else
            {
                _onStarted = Action;
            }
        }
    }
}
