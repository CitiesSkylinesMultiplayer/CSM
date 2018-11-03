using ColossalFramework;
using ColossalFramework.UI;
using CSM.Helpers;
using CSM.Networking;
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
        private UILabel _localIP;
        private UILabel _externalIP;

        private UIButton _createButton;
        private UIButton _closeButton;

        public override void Start()
        {
            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            name = "MPHostGamePanel";
            color = new Color32(110, 110, 110, 250);

            // Grab the view for calculating width and height of game
            var view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(view.fixedWidth / 2.0f - 180.0f, view.fixedHeight / 2.0f - 250.0f);

            width = 360;
            height = 530;

            // Title Label
            this.CreateTitleLabel("Host Server", new Vector2(120, -20));

            // Port Label
            this.CreateLabel("Port:", new Vector2(10, -65));
            // Port field
            _portField = this.CreateTextField("4230", new Vector2(10, -90));
            _portField.numericalOnly = true;

            // Password label
            this.CreateLabel("Password (Optional):", new Vector2(10, -145));
            // Password field
            _passwordField = this.CreateTextField("", new Vector2(10, -170));
            _passwordField.isPasswordField = true;

            // Username label
            this.CreateLabel("Own username:", new Vector2(10, -225));
            // Username field
            _usernameField = this.CreateTextField("", new Vector2(10, -250));

            _connectionStatus = this.CreateLabel("", new Vector2(10, -310));
            _connectionStatus.textAlignment = UIHorizontalAlignment.Center;
            _connectionStatus.textColor = new Color32(255, 0, 0, 255);

            // Request IP adresses async
            new Thread(RequestIPs).Start();

            // Create Local IP Label
            _localIP = this.CreateLabel("", new Vector2(10, -330));
            _localIP.textAlignment = UIHorizontalAlignment.Center;

            // Create External IP Label
            _externalIP = this.CreateLabel("", new Vector2(10, -350));
            _externalIP.textAlignment = UIHorizontalAlignment.Center;

            // Create Server Button
            _createButton = this.CreateButton("Create Server", new Vector2(10, -390));
            _createButton.eventClick += OnCreateServerClick;

            // Close this dialog
            _closeButton = this.CreateButton("Cancel", new Vector2(10, -460));
            _closeButton.eventClick += (component, param) =>
            {
                isVisible = false;
            };
        }

        private void RequestIPs()
        {
            // Request ips
            string sLocalIP = IPAddress.GetLocalIPAddress();
            string sExternalIP = IPAddress.GetExternalIPAddress();

            // Change gui attributes in main thread
            Singleton<SimulationManager>.instance.m_ThreadingWrapper.QueueMainThread(() =>
            {
                _localIP.textColor = sLocalIP.Equals("Not found") ? new Color32(255, 0, 0, 255) : new Color32(0, 255, 0, 255);
                _localIP.text = string.Format("Local IP: {0}", sLocalIP);

                _externalIP.textColor = sExternalIP.Equals("Not found") ? new Color32(255, 0, 0, 255) : new Color32(0, 255, 0, 255);
                _externalIP.text = string.Format("External IP: {0}", sExternalIP);
            });
        }

        /// <summary>
        ///     Check inputs and create server
        /// </summary>
        private void OnCreateServerClick(UIComponent uiComponent, UIMouseEventParameter eventParam)
        {
            _connectionStatus.textColor = new Color32(255, 255, 0, 255);
            _connectionStatus.text = "Setting up server...";

            // Port must be filled in and be a number
            if (string.IsNullOrEmpty(_portField.text) || !int.TryParse(_portField.text, out var port))
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Port must be a number";
                return;
            }

            // Check port is valid to use
            if (int.Parse(_portField.text) < 1 || int.Parse(_portField.text) > 49151)
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Invalid port. Choose from a range of 1 - 49151";
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
                _connectionStatus.text = "Already Running Server";
                return;
            }

            // Start server and check for errors
            if (MultiplayerManager.Instance.StartGameServer(int.Parse(_portField.text), _passwordField.text, _usernameField.text) != true)
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Error: Could not start server";
                return;
            }

            // Clear warnings/errors and hide panel
            _connectionStatus.text = "";
            isVisible = false;
        }
    }
}