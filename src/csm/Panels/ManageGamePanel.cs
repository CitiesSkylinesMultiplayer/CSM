using ColossalFramework.UI;
using CSM.Helpers;
using CSM.Networking;
using UnityEngine;

namespace CSM.Panels
{
    public class ManageGamePanel : UIPanel
    {
        private UITextField _portField;
        private UITextField _localIpField;
        private UITextField _externalIpField;

        private UIButton _closeButton;

        private string _portVal, _localIpVal, _externalIpVal;

        public override void Start()
        {
            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            color = new Color32(110, 110, 110, 250);

            width = 360;
            height = 445;
            relativePosition = PanelManager.GetCenterPosition(this);

            // Title Label
            UILabel title = this.CreateTitleLabel("Manage Server", new Vector2(100, -20));

            // Port label
            this.CreateLabel("Port:", new Vector2(10, -75));

            // Port field
            _portVal = MultiplayerManager.Instance.CurrentServer.Config.Port.ToString();
            _portField = this.CreateTextField(_portVal, new Vector2(10, -100));
            _portField.selectOnFocus = true;
            _portField.eventTextChanged += (ui, value) =>
            {
                _portField.text = _portVal;
            };

            // Local IP label
            this.CreateLabel("Local IP:", new Vector2(10, -150));

            // Local IP field
            _localIpVal = IpAddress.GetLocalIpAddress();
            _localIpField = this.CreateTextField(_localIpVal, new Vector2(10, -175));
            _localIpField.selectOnFocus = true;
            _localIpField.eventTextChanged += (ui, value) =>
            {
                _localIpField.text = _localIpVal;
            };

            // External IP label
            this.CreateLabel("External IP:", new Vector2(10, -225));

            // External IP field
            _externalIpVal = IpAddress.GetExternalIpAddress();
            _externalIpField = this.CreateTextField(_externalIpVal, new Vector2(10, -250));
            _externalIpField.selectOnFocus = true;
            _externalIpField.eventTextChanged += (ui, value) =>
            {
                _externalIpField.text = _externalIpVal;
            };

            // Close this dialog
            _closeButton = this.CreateButton("Close", new Vector2(10, -375));
            _closeButton.eventClick += (component, param) =>
            {
                isVisible = false;
            };

            eventVisibilityChanged += (component, visible) =>
            {
                if (!visible)
                    return;

                _portVal = MultiplayerManager.Instance.CurrentServer.Config.Port.ToString();
                _portField.text = _portVal;
            };
        }
    }
}
