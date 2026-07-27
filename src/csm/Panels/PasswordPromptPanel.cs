using ColossalFramework.UI;
using CSM.API.Commands;
using CSM.Helpers;
using CSM.Networking;
using UnityEngine;

namespace CSM.Panels
{
    /// <summary>
    ///     A focused password-entry prompt used when joining a password-protected
    ///     server (from the public server browser, or a retry after JoinByToken
    ///     failed with a password-related error). Deliberately doesn't expose the
    ///     target token/address in the UI - a browsed server's connection details
    ///     weren't something the player entered themselves, so there's no need to
    ///     display them. Delegates the actual connection attempt to
    ///     JoinGamePanel.JoinByToken.
    /// </summary>
    public class PasswordPromptPanel : UIPanel
    {
        private UITextField _usernameField;
        private UITextField _passwordField;
        private UILabel _connectionStatus;
        private UIButton _connectButton;
        private UIButton _cancelButton;

        private string _targetToken;

        public override void Start()
        {
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            color = new Color32(110, 110, 110, 255);

            width = 360;
            height = 335;
            relativePosition = PanelManager.GetCenterPosition(this);

            this.CreateTitleLabel("Password Required", new Vector2(80, -20));

            this.CreateLabel("This server requires a password to join.", new Vector2(10, -60), 340);

            this.CreateLabel("Username:", new Vector2(10, -95));
            _usernameField = this.CreateTextField("", new Vector2(10, -120));

            this.CreateLabel("Password:", new Vector2(10, -160));
            _passwordField = this.CreateTextField("", new Vector2(10, -185));
            _passwordField.isPasswordField = true;

            _connectionStatus = this.CreateLabel("", new Vector2(10, -225));
            _connectionStatus.textAlignment = UIHorizontalAlignment.Center;
            _connectionStatus.textColor = new Color32(255, 0, 0, 255);

            _connectButton = this.CreateButton("Connect", new Vector2(10, -265), 165);
            _connectButton.eventClick += OnConnectClick;

            _cancelButton = this.CreateButton("Cancel", new Vector2(185, -265), 165);
            _cancelButton.eventClick += (component, param) =>
            {
                isVisible = false;
            };
        }

        public override void Update()
        {
            if (isVisible && Input.GetKeyDown(KeyCode.Escape))
            {
                isVisible = false;
            }
        }

        /// <summary>
        ///     Shows the prompt for the given target server token. The token is
        ///     kept only in memory here, never rendered in the UI.
        /// </summary>
        /// <param name="token">The server's NAT-relay token.</param>
        /// <param name="username">The username to pre-fill (editable).</param>
        /// <param name="errorMessage">
        ///     An optional error to display, e.g. when re-showing this prompt
        ///     after a wrong password was entered.
        /// </param>
        public void Show(string token, string username, string errorMessage = null)
        {
            _targetToken = token;
            _usernameField.text = username;
            _passwordField.text = "";

            if (!string.IsNullOrEmpty(errorMessage))
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = errorMessage;
            }
            else
            {
                _connectionStatus.text = "";
            }

            isVisible = true;
        }

        private void OnConnectClick(UIComponent component, UIMouseEventParameter param)
        {
            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Already Running Server";
                return;
            }

            if (string.IsNullOrEmpty(_usernameField.text))
            {
                _connectionStatus.textColor = new Color32(255, 0, 0, 255);
                _connectionStatus.text = "Invalid Username";
                return;
            }

            isVisible = false;
            JoinGamePanel.JoinByToken(_targetToken, _usernameField.text, _passwordField.text);
        }
    }
}
