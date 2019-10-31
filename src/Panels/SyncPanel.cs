using ColossalFramework;
using ColossalFramework.UI;
using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Status;
using System.Threading;
using UnityEngine;

namespace CSM.Panels
{
    class SyncPanel : UIPanel
    {
        private ClientStatus _lastStatus;

        private UILabel _statusLabel;

        private UIButton _cancelButton;

        public override void Start()
        {
            backgroundSprite = "GenericPanel";
            name = "MPSyncPanel";
            color = new Color32(110, 110, 110, 255);

            // Grab the view for calculating width and height of game
            UIView view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(0, 0);

            width = view.fixedWidth;
            height = view.fixedHeight;

            // Connecting Status
            _statusLabel = this.CreateTitleLabel("", new Vector2(0, 0));
            _lastStatus = MultiplayerManager.Instance.CurrentClient.Status;
            UpdateStatus();

            _cancelButton = this.CreateButton("Cancel", new Vector2((width / 2f) - 170f, -(height / 2f) - 60f));
            _cancelButton.eventClick += OnCancelButtonClick;

            new Thread(() => 
            {
                ClientStatus CurrentStatus = MultiplayerManager.Instance.CurrentClient.Status;
                while (CurrentStatus == ClientStatus.Downloading || CurrentStatus == ClientStatus.Loading)
                {
                    Thread.Sleep(500);
                    if (_lastStatus != CurrentStatus) 
                    {
                        _lastStatus = CurrentStatus;
                        Singleton<SimulationManager>.instance.m_ThreadingWrapper.QueueMainThread(() =>
                        {
                            UpdateStatus();
                        });
                    }
                    CurrentStatus = MultiplayerManager.Instance.CurrentClient.Status;
                }
                Singleton<SimulationManager>.instance.m_ThreadingWrapper.QueueMainThread(() =>
                {
                    isVisible = false;
                });
            }).Start();
        }

        public void UpdateStatus()
        {
            _statusLabel.position = new Vector2(0, 60);
            _statusLabel.text = GetStatusMessage();
            float w = _statusLabel.width;
            _statusLabel.position = new Vector2((width / 2f) - (w / 2f), -(height / 2f) + 60f);
        }

        private void OnCancelButtonClick(UIComponent uiComponent, UIMouseEventParameter eventParam)
        {
            isVisible = false;
            MultiplayerManager.Instance.CurrentClient.Disconnect();
        }

        private string GetStatusMessage()
        {
            switch (_lastStatus) {
                case ClientStatus.Downloading:
                    return "Downloading save game...";
                case ClientStatus.Loading:
                    return "Loading save game...";
                case ClientStatus.Connected:
                case ClientStatus.Connecting:
                case ClientStatus.Disconnected:
                default:
                    return "Invalid status";
            }
        }
    }
}
