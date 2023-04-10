using ColossalFramework.UI;
using CSM.Commands;
using CSM.Commands.Data.Internal;
using CSM.Commands.Handler.Internal;
using CSM.Helpers;
using CSM.Networking;
using UnityEngine;

namespace CSM.Panels
{
    public class JoinStatusPanel : UIPanel
    {
        private UILabel _statusLabel;

        private UIButton _cancelButton;

        public bool IsSelf { get; set; }

        public bool IsFirstJoin { get; set; }

        public string JoiningUsername { get; set; }

        public override void Start()
        {
            backgroundSprite = "GenericPanel";
            name = nameof(JoinStatusPanel);
            color = new Color32(110, 110, 110, 220);

            relativePosition = new Vector3(0, 0);

            width = GetUIView().GetScreenResolution().x;
            height = GetUIView().GetScreenResolution().y;

            // Connecting Status
            _statusLabel = this.CreateTitleLabel("", new Vector2(0, 60));

            // Cancel button only while first join
            _cancelButton = this.CreateButton("Cancel", new Vector2((width / 2f) - 170f, -(height / 2f) - 60f));
            _cancelButton.eventClick += OnCancelButtonClick;
            _cancelButton.isVisible = false;
        }

        public override void Update()
        {
            if (!isVisible)
            {
                return;
            }

            _statusLabel.text = GetStatusMessage();
            float w = _statusLabel.width;
            Vector3 pos = new Vector2((width - w) / 2f, -(height / 2f) + 60f);
            if (!_statusLabel.position.Equals(pos))
            {
                _statusLabel.position = pos;
            }

            if (_cancelButton.isVisible != IsFirstJoin)
            {
                _cancelButton.isVisible = IsFirstJoin;
            }
        }

        private void OnCancelButtonClick(UIComponent uiComponent, UIMouseEventParameter eventParam)
        {
            MultiplayerManager.Instance.StopEverything();
        }

        private string GetStatusMessage()
        {
            if (IsSelf)
            {
                string display = IsFirstJoin ? "Downloading save game..." : "Re-downloading save game...";
                WorldTransferHandler worldTransferHandler = CommandInternal.Instance.GetCommandHandler<WorldTransferCommand, WorldTransferHandler>();
                WorldFileCombiner worldFileCombiner = worldTransferHandler.WorldFileCombiner;
                if (worldFileCombiner != null)
                {
                    double totalBytes = worldFileCombiner.GetTotalBytes();
                    double transmitBytes = totalBytes - worldFileCombiner.RemainingBytes;
                    string total = FileSizeHelper.FormatFilesizeIEC(totalBytes, 0);
                    string transmit = FileSizeHelper.FormatFilesizeIEC(transmitBytes, 0);
                    double percentage = transmitBytes / totalBytes * 100;
                    display += $"\n{percentage:0.#} %\n{transmit} / {total}";
                }
                else
                {
                    display += "\n0 %";
                }
                return display;
            }
            else
            {
                return JoiningUsername + " is joining...";
            }
        }
    }
}
