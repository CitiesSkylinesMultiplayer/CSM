using System.Threading;
using ColossalFramework;
using ColossalFramework.UI;
using CSM.GS.Commands;
using CSM.GS.Commands.Data.ApiServer;
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
        private UITextField _vpnIpField;
        private UILabel _vpnLabel;
        private UILabel _portState;

        private UIButton _closeButton;
        private UIButton _troubleshootingButton;

        private int _portVal;
        private string _localIpVal, _externalIpVal, _vpnIpVal;

        public override void Start()
        {
            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            color = new Color32(110, 110, 110, 250);

            width = 360;
            height = 420;
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

            _portState = this.CreateLabel("", new Vector2(120, -225));
            _troubleshootingButton = this.CreateButton("?", new Vector2(325, -225), 25, 20);
            _troubleshootingButton.isVisible = false;
            _troubleshootingButton.eventClick += (component, param) =>
            {
                MessagePanel panel = PanelManager.ShowPanel<MessagePanel>();
                panel.DisplayTroubleshooting(true, _portVal, _vpnIpVal != null);
            };

            // External IP field
            _externalIpField = this.CreateTextField("", new Vector2(10, -250));
            _externalIpField.selectOnFocus = true;
            _externalIpField.eventTextChanged += (ui, value) =>
            {
                _externalIpField.text = _externalIpVal;
            };

            // VPN IP label
            _vpnLabel = this.CreateLabel("Hamachi IP:", new Vector2(10, -300));
            _vpnLabel.isVisible = false;
            _vpnIpField = this.CreateTextField("", new Vector2(10, -325));
            _vpnIpField.selectOnFocus = true;
            _vpnIpField.isVisible = false;
            _vpnIpField.eventTextChanged += (ui, value) =>
            {
                _vpnIpField.text = _vpnIpVal;
            };

            // Close this dialog
            _closeButton = this.CreateButton("Close", new Vector2(10, _vpnIpVal == null ? -340 : -415));
            _closeButton.eventClick += (component, param) =>
            {
                isVisible = false;
            };

            new Thread(UpdateWindow).Start();

            eventVisibilityChanged += (component, visible) =>
            {
                if (!visible)
                    return;

                new Thread(UpdateWindow).Start();
            };
        }

        private void UpdateWindow()
        {
            _vpnIpVal = IpAddress.GetVPNIpAddress();
            _localIpVal = IpAddress.GetLocalIpAddress();
            _externalIpVal = IpAddress.GetExternalIpAddress();

            // Check if port is reachable
            ApiCommand.Instance.SendToApiServer(new PortCheckRequestCommand { Port = _portVal });

            Singleton<SimulationManager>.instance.m_ThreadingWrapper.QueueMainThread(() =>
            {
                _localIpField.text = _localIpVal;
                _externalIpField.text = _externalIpVal;

                _portState.text = "Checking port...";
                _portState.textColor = new Color32(255, 255, 0, 255);
                _portState.tooltip = "Checking if port is reachable from the internet...";
                _troubleshootingButton.isVisible = false;

                _portVal = MultiplayerManager.Instance.CurrentServer.Config.Port;
                _portField.text = _portVal.ToString();

                height = _vpnIpVal == null ? 420 : 495;
                _closeButton.position = new Vector2(10, _vpnIpVal == null ? -340 : -415);
                if (_vpnIpVal == null)
                {
                    _vpnLabel.isVisible = false;
                    _vpnIpField.isVisible = false;
                }
                else
                {
                    _vpnLabel.isVisible = true;
                    _vpnIpField.isVisible = true;
                    _vpnIpField.text = _vpnIpVal;
                }
            });
        }

        public void SetPortState(PortCheckResultCommand res)
        {
            Singleton<SimulationManager>.instance.m_ThreadingWrapper.QueueMainThread(() =>
            {
                if (_portState == null)
                    return;

                switch (res.State)
                {
                    case PortCheckResult.Unreachable:
                        _portState.text = "Port is not reachable!";
                        _portState.textColor = new Color32(255, 0, 0, 255);
                        _portState.tooltip = res.Message;
                        _troubleshootingButton.isVisible = true;
                        break;
                    case PortCheckResult.Reachable:
                        _portState.text = "Port is reachable!";
                        _portState.textColor = new Color32(0, 255, 0, 255);
                        _portState.tooltip = "";
                        break;
                    case PortCheckResult.Error:
                    default:
                        _portState.text = "Failed to check port";
                        _portState.textColor = new Color32(255, 0, 0, 255);
                        _portState.tooltip = res.Message;
                        break;
                }
            });
        }
    }
}
