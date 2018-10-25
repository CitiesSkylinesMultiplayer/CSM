using ColossalFramework.UI;
using CSM.Helpers;
using CSM.Networking;
using System.Collections.Generic;
using UnityEngine;

namespace CSM.Panels
{
    public class PlayerListPanel : UIScrollablePanel
    {
        private UIButton _closeButton;

        private readonly List<UILabel> _playerLines = new List<UILabel>();

        public override void Start()
        {
            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            name = "MPPlayerListPanel";
            color = new Color32(110, 110, 110, 250);

            // Grab the view for calculating width and height of game
            var view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(view.fixedWidth / 2.0f - 180.0f, view.fixedHeight / 2.0f - 240.0f);

            width = 360;
            height = 480;

            // Title Label
            this.CreateTitleLabel("Connected Players", new Vector2(100, -20));

            // Close this dialog
            _closeButton = this.CreateButton("Close", new Vector2(10, -410));
            _closeButton.eventClick += (component, param) =>
            {
                isVisible = false;
            };

            GenerateList();

            this.eventVisibilityChanged += (component, visible) =>
            {
                if (!visible)
                    return;

                GenerateList();
            };
        }

        private void GenerateList()
        {
            foreach (UILabel label in _playerLines)
            {
                RemoveUIComponent(label);
            }
            _playerLines.Clear();

            int y = -75;
            foreach (string player in MultiplayerManager.Instance.PlayerList)
            {
                UILabel label = this.CreateLabel(player, new Vector2(10, y));
                y -= 30;
                _playerLines.Add(label);
            }
        }
    }
}