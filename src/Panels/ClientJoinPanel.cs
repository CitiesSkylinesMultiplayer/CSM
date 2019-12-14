using ColossalFramework;
using ColossalFramework.UI;
using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Status;
using System.Threading;
using UnityEngine;

namespace CSM.Panels
{
    class ClientJoinPanel : UIPanel
    {
        private ClientStatus _lastStatus;

        private UILabel _statusLabel;

        public override void Start()
        {
            backgroundSprite = "GenericPanel";
            name = "MPClientJoinPanel";
            color = new Color32(110, 110, 110, 220);

            // Grab the view for calculating width and height of game
            UIView view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(0, 0);

            width = view.fixedWidth;
            height = view.fixedHeight;

            // Connecting Status
            _statusLabel = this.CreateTitleLabel("", new Vector2(0, 0));
            UpdateText("A client is joining...");

            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
            {
                StartCheck();
            }
        }

        private void UpdateText(string message) 
        {
            _statusLabel.position = new Vector2(0, 60);
            _statusLabel.text = message;
            float w = _statusLabel.width;
            _statusLabel.position = new Vector2((width / 2f) - (w / 2f), -(height / 2f) + 60f);
        }

        private string GetStatusMessage()
        {
            switch (_lastStatus)
            {
                case ClientStatus.Downloading:
                    return "Client is downloading save game...";
                case ClientStatus.Loading:
                    return "Client is loading save game...";
                case ClientStatus.Connected:
                case ClientStatus.Connecting:
                case ClientStatus.Disconnected:
                default:
                    return "Invalid status";
            }
        }

        private ClientStatus GetStatus()
        {
            foreach (Player p in MultiplayerManager.Instance.CurrentServer.ConnectedPlayers.Values)
            {
                if (p.Status != ClientStatus.Connected)
                {
                    return p.Status;
                }
            }
            return ClientStatus.Connected;
        }

        public void StartCheck() 
        {
            _lastStatus = GetStatus();
            UpdateText(GetStatusMessage());

            new Thread(() =>
            {
                ClientStatus CurrentStatus = GetStatus();
                while (CurrentStatus == ClientStatus.Downloading || CurrentStatus == ClientStatus.Loading)
                {
                    Thread.Sleep(100);
                    if (_lastStatus != CurrentStatus)
                    {
                        _lastStatus = CurrentStatus;
                        Singleton<SimulationManager>.instance.m_ThreadingWrapper.QueueMainThread(() =>
                        {
                            UpdateText(GetStatusMessage());
                        });
                    }
                    CurrentStatus = GetStatus();
                }
                Singleton<SimulationManager>.instance.m_ThreadingWrapper.QueueMainThread(() =>
                {
                    isVisible = false;
                });
            }).Start();
        }
    }
}
