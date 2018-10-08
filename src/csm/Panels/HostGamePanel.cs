using ColossalFramework.UI;
using CSM.Helpers;
using CSM.Networking;
using UnityEngine;

namespace CSM.Panels
{
    public class HostGamePanel : UIPanel
    {

        private UITextField _ipAddressField;
        private UITextField _portField;
        private UITextField _passwordField;

        private UILabel _connectionStatus;

        private UIButton _createButton;
        private UIButton _closeButton;

        public override void Start()
        {
            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            name = "MPHostGamePanel";
            color = new Color32(110, 110, 110, 250);

            // Grab the view for caculating width and height of game
            var view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(view.fixedWidth / 2.0f - 180.0f, view.fixedHeight / 2.0f - 200.0f);

            width = 360;
            height = 480;

            // Title Label
            this.CreateTitleLabel("Host custom server", new Vector2(80, -20));

            // IP Address Label
            this.CreateLabel("IP Address:", new Vector3(10, -75, 0));

            // IP Address field (disabled)
            _ipAddressField = this.CreateTextField("localhost", new Vector2(10, -100));
            _ipAddressField.readOnly = true;

            // Port Label
            this.CreateLabel("Port:", new Vector2(10, -150));

            // Port field
            _portField = this.CreateTextField("4230", new Vector2(10, -180));

            // Password label
            this.CreateLabel("Password:", new Vector2(10, -230));

            // Password field
            _passwordField = this.CreateTextField("not available yet", new Vector2(10, -260));
            _passwordField.readOnly = true; // Todo: Allow password

            // Create Server Button
            _createButton = this.CreateButton("Create server", new Vector2(10, -340));
            _createButton.eventClick += OnCreateServerClick;

            // Close this dialog
            _closeButton = this.CreateButton("Cancel", new Vector2(10, -410));
            _closeButton.eventClick += (component, param) =>
            {
                isVisible = false;
            };

            _connectionStatus = this.CreateLabel("", new Vector2(10, -310));
            _connectionStatus.textAlignment = UIHorizontalAlignment.Center;
            _connectionStatus.textColor = new Color32(255, 0, 0, 255);
        }


        /// <summary>
        ///     Check inputs and create server
        /// </summary>
        private void OnCreateServerClick(UIComponent uiComponent, UIMouseEventParameter eventParam)
        {
            _connectionStatus.textColor = new Color32(255, 255, 0, 255);
            _connectionStatus.text = "Setting up server...";

            // Port must be filled in or be a number
            if (string.IsNullOrEmpty(_portField.text) || !int.TryParse(_portField.text, out var port))
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Port must be a number";
                return;
            }

            // Check port is valid to use
            if(int.Parse(_portField.text) < 1 || int.Parse(_portField.text) > 49151)
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Invalid port. Choose from range 1 - 49151";
                return;
            }

            /// <summary>
            ///     TODO: Password validation
            ///     Invalid password or password contains spaces
            /// </summary>

            //if (!string.IsNullOrEmpty(_passwordField.text) /*&& (!int.TryParse(_portField.text, out var ) || _passwordField.text == " ")*/)
            //{
            //    _connectionStatus.textColor = new Color32(255, 0, 0, 255);
            //    _connectionStatus.text = "Invalid Password. Must be text or numeric.";
            //    return;
            //}

            // Check already running server
            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Already Running Server";
                return;
            }

            // Start server and check for errors
            if(MultiplayerManager.Instance.StartGameServer(int.Parse(_portField.text)) != true)
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Error: Could not start server";
                return;
            }

            // Crear warnings/errors and hide panel
            _connectionStatus.text = "";
            isVisible = false;
        }
    }
}