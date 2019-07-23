using ColossalFramework;
using ColossalFramework.UI;
using CSM.Helpers;
using CSM.Networking;
using UnityEngine;
using CSM.Localisation;

namespace CSM.Panels
{
    public class ManageGamePanel : UIPanel
    {
        private UITextField _portField;
        private UITextField _localIpField;
        private UITextField _externalIpField;

        private UIButton _closeButton;
        private UIButton _extIP_PortButton;

        private string _portVal, _localIpVal, _externalIpVal;

        public override void Start()
        {
            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            name = "MPManageGamePanel";
            color = new Color32(110, 110, 110, 250);

            ChatLogPanel.ActiveWindows.Add(name);

            // Grab the view for calculating width and height of game
            UIView view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(view.fixedWidth / 2.0f - 180.0f, view.fixedHeight / 2.0f - 240.0f);

            width = 360;
            height = 445;

            // Title Label
            UILabel title = this.CreateTitleLabel(Translation.PullTranslation("ManageServer"), new Vector2(100, -20));

            // Port label
            this.CreateLabel(Translation.PullTranslation("Port"), new Vector2(10, -75));

            // Port field
            _portVal = MultiplayerManager.Instance.CurrentServer.Config.Port.ToString();
            _portField = this.CreateTextField(_portVal, new Vector2(10, -100));
            _portField.selectOnFocus = true;
            _portField.eventTextChanged += (UIComponent ui, string val) =>
            {
                _portField.text = _portVal;
            };

            // Local IP label
            this.CreateLabel(Translation.PullTranslation("InternalIP"), new Vector2(10, -150));

            // Local IP field
            _localIpVal = IPAddress.GetLocalIPAddress();
            _localIpField = this.CreateTextField(_localIpVal, new Vector2(10, -175));
            _localIpField.selectOnFocus = true;
            _localIpField.eventTextChanged += (UIComponent ui, string val) =>
            {
                _localIpField.text = _localIpVal;
            };

            // External IP label
            this.CreateLabel(Translation.PullTranslation("ExternalIP"), new Vector2(10, -225));

            // External IP field
            _externalIpVal = IPAddress.GetExternalIPAddress();
            _externalIpField = this.CreateTextField(_externalIpVal, new Vector2(10, -250));
            _externalIpField.selectOnFocus = true;
            _externalIpField.eventTextChanged += (UIComponent ui, string val) =>
            {
                _externalIpField.text = _externalIpVal;
            };

            // Create Clipboard Button
            _extIP_PortButton = this.CreateButton("", new Vector2(10, -300));
            _extIP_PortButton.text = Translation.PullTranslation("CopyToClipboard");
            _extIP_PortButton.eventClick += (component, param) =>
            {
                Clipboard.text = $"{Translation.PullTranslation("ExternalIP")}: {_externalIpField.text} / {Translation.PullTranslation("Port")}: {_portField.text}";
            };

            // Close this dialog
            _closeButton = this.CreateButton(Translation.PullTranslation("Close"), new Vector2(10, -375));
            _closeButton.eventClick += (component, param) =>
            {
                isVisible = false;
            };

            eventVisibilityChanged += (component, visible) =>
            {
                if (!visible)
                    return;

                _portField.text = MultiplayerManager.Instance.CurrentServer.Config.Port.ToString();
            };
        }
    }
}