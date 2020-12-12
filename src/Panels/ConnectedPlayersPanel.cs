using ColossalFramework.UI;
using CSM.Helpers;
using CSM.Networking;
using System.Collections.Generic;
using UnityEngine;

namespace CSM.Panels
{
    public class ConnectedPlayersPanel : UIPanel
    {
        private UIButton _closeButton;

        private List<UILabel> _playerLabels;
        private List<UIButton> _kickButtons;

        private int _playerCountLastUpdate;
        private bool _playerListChanged;

        public override void Start()
        {
            _playerLabels = new List<UILabel>();
            _kickButtons = new List<UIButton>();

            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            color = new Color32(110, 110, 110, 250);

            // Grab the view for calculating width and height of game
            UIView view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(view.fixedWidth / 2.0f - 180.0f, view.fixedHeight / 2.0f - 240.0f);

            width = 360;
            height = 445;

            // Title Label
            UILabel title = this.CreateTitleLabel("Connected Players", new Vector2(80, -20));

            // Close this dialog
            _closeButton = this.CreateButton("Close", new Vector2(10, -375));
            _closeButton.eventClick += (component, param) =>
            {
                isVisible = false;
            };

            base.Start();
        }

        public override void Update()
        {
            int playerCountThisUpdate = MultiplayerManager.Instance.PlayerList.Count;

            // This assumes that two players cannot join at once, but that seems reasonable
            if (_playerCountLastUpdate != playerCountThisUpdate)
            {
                _playerCountLastUpdate = playerCountThisUpdate;
                _playerListChanged = true;
            }

            // Update the list of players and kick buttons if anything has changed
            if (isVisible && _playerListChanged)
            {
                // Destroy Unity reference
                foreach (UILabel label in _playerLabels)
                {
                    Destroy(label);
                }

                foreach (UIButton button in _kickButtons)
                {
                    Destroy(button);
                }

                // Clear managed references
                _playerLabels.Clear();
                _kickButtons.Clear();

                // Kick button margins for UI
                int topOffset = -75;
                int currentPlayerOffset = 0;
                int playerOffset = -30;

                // Enable Host to see and kick all players
                if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
                {
                    // List all the players
                    foreach (string player in MultiplayerManager.Instance.PlayerList)
                    {
                        _playerLabels.Add(this.CreateLabel(player, new Vector2(10, topOffset + currentPlayerOffset)));

                        if (player != MultiplayerManager.Instance.CurrentServer.HostPlayer.Username)
                        {
                            UIButton button = this.CreateButton("Kick", new Vector2(200, topOffset + currentPlayerOffset), 100, 30);

                            button.eventClick += (component, param) =>
                            {
                                MultiplayerManager.Instance.CurrentServer.GetPlayerByUsername(player).NetPeer.Disconnect();
                            };

                            _kickButtons.Add(button);
                        }

                        currentPlayerOffset += playerOffset;
                    }
                }
                // Enable Client to see all players
                else if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
                {
                    // List all the players
                    foreach (string player in MultiplayerManager.Instance.PlayerList)
                    {
                        _playerLabels.Add(this.CreateLabel(player, new Vector2(10, topOffset + currentPlayerOffset)));

                        currentPlayerOffset += playerOffset;
                    }
                }

                _playerListChanged = false;
            }

            base.Update();
        }
    }
}
