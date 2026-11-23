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
        private string _username = "";
        private string _errorMessage = null;

        public override void Start()
        {
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            color = new Color32(110, 110, 110, 255);

            width = 380;
            height = 390;
            relativePosition = PanelManager.GetCenterPosition(this);

            this.CreateTitleLabel("Password Required", new Vector2(80, -20));

            this.CreateLabel("This server requires a password to join.", new Vector2(10, -70), 340);

            this.CreateLabel("Username:", new Vector2(10, -110));
            _usernameField = this.CreateTextField(_username, new Vector2(10, -140));

            this.CreateLabel("Password:", new Vector2(10, -190));
            _passwordField = this.CreateTextField("", new Vector2(10, -220));
            _passwordField.isPasswordField = true;

            _connectionStatus = this.CreateLabel(_errorMessage, new Vector2(10, -270));
            _connectionStatus.textAlignment = UIHorizontalAlignment.Center;
            _connectionStatus.textColor = new Color32(255, 0, 0, 255);

            _connectButton = this.CreateButton("Connect", new Vector2(10, -320), 165);
            _connectButton.eventClick += OnConnectClick;

            _cancelButton = this.CreateButton("Cancel", new Vector2(185, -320), 165);
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
            _username = username;
            _errorMessage = errorMessage ?? "";

            if (_usernameField == null)
            {
                // Not yet initialized
                return;
            }
            _usernameField.text = username;
            _passwordField.text = "";

            _connectionStatus.text = _errorMessage;

            isVisible = true;
        }

        private void OnConnectClick(UIComponent component, UIMouseEventParameter param)
        {
            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
            {
                _connectionStatus.text = "Already Running Server";
                return;
            }

            if (string.IsNullOrEmpty(_usernameField.text))
            {
                _connectionStatus.text = "Invalid Username";
                return;
            }

            isVisible = false;
            JoinGamePanel.JoinByToken(_targetToken, _usernameField.text, _passwordField.text);
        }
    }
}
