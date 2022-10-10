using System.Net;
using System.Threading;
using ColossalFramework;
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
        private UILabel _portState;

        private UIButton _closeButton;

        private int _portVal;
        private string _localIpVal, _externalIpVal, _vpnIpVal;

        public override void Start()
        {
            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            color = new Color32(110, 110, 110, 250);

            _vpnIpVal = IpAddress.GetVPNIpAddress();

            width = 360;
            height = _vpnIpVal == null ? 420 : 495;
            relativePosition = PanelManager.GetCenterPosition(this);

            // Title Label
            this.CreateTitleLabel("Manage Server", new Vector2(100, -20));

            // Port label
            this.CreateLabel("Port:", new Vector2(10, -75));

            // Port field
            _portVal = MultiplayerManager.Instance.CurrentServer.Config.Port;
            _portField = this.CreateTextField(_portVal.ToString(), new Vector2(10, -100));
            _portField.selectOnFocus = true;
            _portField.eventTextChanged += (ui, value) =>
            {
                _portField.text = _portVal.ToString();
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

            _portState = this.CreateLabel("", new Vector2(140, -225));

            // External IP field
            _externalIpVal = IpAddress.GetExternalIpAddress();
            _externalIpField = this.CreateTextField(_externalIpVal, new Vector2(10, -250));
            _externalIpField.selectOnFocus = true;
            _externalIpField.eventTextChanged += (ui, value) =>
            {
                _externalIpField.text = _externalIpVal;
            };

            if (_vpnIpVal != null)
            {
                // VPN IP label
                this.CreateLabel("Hamachi IP:", new Vector2(10, -300));

                // VPN IP field
                UITextField vpnIpField = this.CreateTextField(_vpnIpVal, new Vector2(10, -325));
                vpnIpField.selectOnFocus = true;
                vpnIpField.eventTextChanged += (ui, value) =>
                {
                    vpnIpField.text = _vpnIpVal;
                };
            }

            // Close this dialog
            _closeButton = this.CreateButton("Close", new Vector2(10, _vpnIpVal == null ? -340 : -415));
            _closeButton.eventClick += (component, param) =>
            {
                isVisible = false;
            };

            new Thread(CheckPort).Start();

            eventVisibilityChanged += (component, visible) =>
            {
                if (!visible)
                    return;

                _portVal = MultiplayerManager.Instance.CurrentServer.Config.Port;
                _portField.text = _portVal.ToString();
                new Thread(CheckPort).Start();
            };
        }

        private void CheckPort()
        {
            Singleton<SimulationManager>.instance.m_ThreadingWrapper.QueueMainThread(() =>
            {
                _portState.text = "Checking port...";
                _portState.textColor = new Color32(255, 255, 0, 255);
                _portState.tooltip = "Checking if port is reachable from the internet...";
            });

            PortState state = IpAddress.CheckPort(_portVal);
            Singleton<SimulationManager>.instance.m_ThreadingWrapper.QueueMainThread(() =>
            {
                switch (state.status)
                {
                    case HttpStatusCode.ServiceUnavailable: // Could not reach port
                        _portState.text = "Port is not reachable!";
                        _portState.textColor = new Color32(255, 0, 0, 255);
                        _portState.tooltip = state.message;
                        break;
                    case HttpStatusCode.OK: // Success
                        _portState.text = "Port is reachable!";
                        _portState.textColor = new Color32(0, 255, 0, 255);
                        _portState.tooltip = "";
                        break;
                    default: // Something failed
                        _portState.text = "Failed to check port";
                        _portState.textColor = new Color32(255, 0, 0, 255);
                        _portState.tooltip = state.message;
                        break;
                }
            });
        }
    }
}
