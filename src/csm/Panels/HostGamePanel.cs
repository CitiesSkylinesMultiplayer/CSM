using ColossalFramework;
using ColossalFramework.PlatformServices;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Config;
using System.Threading;
using UnityEngine;

namespace CSM.Panels
{
    public class HostGamePanel : UIPanel
    {
        private UITextField _portField;
        private UITextField _passwordField;
        private UITextField _usernameField;

        private UILabel _connectionStatus;
        private UILabel _localIp;
        private UILabel _externalIp;

        private UIButton _createButton;
        private UIButton _closeButton;

        private UICheckBox _passwordBox;
        private UICheckBox _rememberBox;

        private ServerConfig _serverConfig;
        private bool _hasRemembered;

        public override void Start()
        {
            _hasRemembered = ConfigData.Load<ServerConfig>(ref _serverConfig, ConfigData.ServerFile);

            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            color = new Color32(110, 110, 110, 250);

            // Grab the view for calculating width and height of game
            UIView view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(Screen.width / 2.0f - 180.0f, Screen.height / 2.0f - 250.0f);

            width = 360;
            height = 580;

            // Title Label
            this.CreateTitleLabel("Host Server", new Vector2(120, -20));

            // Port Label
            this.CreateLabel("Port:", new Vector2(10, -65));
            // Port field
            _portField = this.CreateTextField(_serverConfig.Port.ToString(), new Vector2(10, -90));
            _portField.numericalOnly = true;

            // Password label
            this.CreateLabel("Password (Optional):", new Vector2(10, -145));
            // Password checkbox
            _passwordBox = this.CreateCheckBox("Show Password", new Vector2(10, -170));
            _passwordBox.isChecked = false;
            // Password field
            _passwordField = this.CreateTextField(_serverConfig.Password, new Vector2(10, -190));
            _passwordField.isPasswordField = true;

            // Username label
            this.CreateLabel("Username:", new Vector2(10, -245));
            // Username field
            _usernameField = this.CreateTextField(_serverConfig.Username, new Vector2(10, -270));
            if (PlatformService.active && PlatformService.personaName != null && _serverConfig.Username == "")
            {
                _usernameField.text = PlatformService.personaName;
            }

            // Remember-Me box
            _rememberBox = this.CreateCheckBox("Remember Me", new Vector2(10, -325));
            _rememberBox.isChecked = _hasRemembered;

            _connectionStatus = this.CreateLabel("", new Vector2(10, -350));
            _connectionStatus.textAlignment = UIHorizontalAlignment.Center;
            _connectionStatus.textColor = new Color32(255, 0, 0, 255);

            // Request IP addresses async
            new Thread(RequestIPs).Start();

            // Create Local IP Label
            _localIp = this.CreateLabel("", new Vector2(10, -380));
            _localIp.textAlignment = UIHorizontalAlignment.Center;

            // Create External IP Label
            _externalIp = this.CreateLabel("", new Vector2(10, -400));
            _externalIp.textAlignment = UIHorizontalAlignment.Center;

            // Create Server Button
            _createButton = this.CreateButton("Create Server", new Vector2(10, -445));
            _createButton.eventClick += OnCreateServerClick;

            // Close this dialog
            _closeButton = this.CreateButton("Cancel", new Vector2(10, -515));
            _closeButton.eventClick += (component, param) =>
            {
                isVisible = false;
            };

            _passwordBox.eventClicked += (component, param) =>
            {
                _passwordField.isPasswordField = !_passwordBox.isChecked;
            };
        }

        public override void Update()
        {
            // Check if escape is pressed and panel is visible.
            if (isVisible && Input.GetKeyDown(KeyCode.Escape))
            {
                isVisible = false;
            }
        }

        private void RequestIPs()
        {
            // Request ips
            string sLocalIp = IpAddress.GetLocalIpAddress();
            string sExternalIp = IpAddress.GetExternalIpAddress();

            // Change gui attributes in main thread
            Singleton<SimulationManager>.instance.m_ThreadingWrapper.QueueMainThread(() =>
            {
                _localIp.textColor = sLocalIp.Equals("Not found") ? new Color32(255, 0, 0, 255) : new Color32(0, 255, 0, 255);
                _localIp.text = $"Local IP: {sLocalIp}";

                _externalIp.textColor = sExternalIp.Equals("Not found") ? new Color32(255, 0, 0, 255) : new Color32(0, 255, 0, 255);
                _externalIp.text = $"External IP: {sExternalIp}";
            });
        }

        /// <summary>
        ///     Check inputs and create server
        /// </summary>
        private void OnCreateServerClick(UIComponent uiComponent, UIMouseEventParameter eventParam)
        {
            _serverConfig = new ServerConfig(System.Int32.Parse(_portField.text), _usernameField.text, _passwordField.text, 0);
            ConfigData.Save<ServerConfig>(ref _serverConfig, ConfigData.ServerFile, _rememberBox.isChecked);

            _connectionStatus.textColor = new Color32(255, 255, 0, 255);
            _connectionStatus.text = "Setting up server...";

            // Port must be filled in and be a number
            if (string.IsNullOrEmpty(_portField.text) || !int.TryParse(_portField.text, out int _))
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Port must be a number";
                return;
            }

            // Check port is valid to use
            if (int.Parse(_portField.text) < 1 || int.Parse(_portField.text) > 49151)
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Invalid port number. (1 - 49151)";
                return;
            }

            // Username must be filled in
            if (string.IsNullOrEmpty(_usernameField.text))
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Invalid username.";
                return;
            }

            // Check already running server
            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Already running server.";
                return;
            }

            // Start server and check for errors
            MultiplayerManager.Instance.StartGameServer(_serverConfig, (success) =>
            {
                ThreadHelper.dispatcher.Dispatch(() =>
                {
                    if (!success)
                    {
                        _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                        _connectionStatus.text = "Could not start server.";
                    }
                    else
                    {
                        // Clear warnings/errors and hide panel
                        _connectionStatus.text = "";
                        isVisible = false;
                    }
                });
            });
        }
    }
}
