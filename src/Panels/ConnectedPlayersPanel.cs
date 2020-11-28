using ColossalFramework.UI;
using CSM.Helpers;
using CSM.Networking;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CSM.Panels
{
    public class ConnectedPlayersPanel : UIPanel
    {
        private UIButton _closeButton;

        private List<UILabel> _playerLabels;
        private List<UIButton> _kickButtons;

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

            eventVisibilityChanged += (component, visible) =>
            {
                if (!visible)
                    return;
            };

            base.Start();
        }

        public override void Update()
        {
            // Update the list of players and the kick buttons
            if (isVisible)
            {
                foreach(UILabel label in _playerLabels)
                {
                    label.Hide();
                    label.Disable();
                }

                int topOffset = -75;
                int currentPlayerOffset = 0;
                int playerOffset = -30;

                // Enable Host to see and kick all players
                if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
                {
                    foreach (UIButton button in _kickButtons)
                    {
                        button.Hide();
                        button.Disable();
                    }

                    List<string> players = MultiplayerManager.Instance.PlayerList.ToList();

                    // List all the players
                    for (int i = 0; i < players.Count; i++)
                    {
                        string player = players[i];

                        if (i < _playerLabels.Count)
                        {
                            _playerLabels[i].text = player;
                            _playerLabels[i].Show();
                            _playerLabels[i].Enable();

                            if (player != MultiplayerManager.Instance.CurrentServer.HostPlayer.Username)
                            {
                                _kickButtons[i].Show();
                                _kickButtons[i].Enable();
                            }

                            _kickButtons[i].eventClick += (component, param) =>
                            {
                                MultiplayerManager.Instance.CurrentServer.ConnectedPlayers.Single(z => z.Value.Username == player).Value.NetPeer.Disconnect();
                            };
                        }
                        else
                        {
                            _playerLabels.Add(this.CreateLabel(player, new Vector2(10, topOffset + currentPlayerOffset)));
                            _kickButtons.Add(this.CreateButton("Kick", new Vector2(200, topOffset + currentPlayerOffset), 100, 30));

                            if (player == MultiplayerManager.Instance.CurrentServer.HostPlayer.Username)
                            {
                                _kickButtons[i].Hide();
                                _kickButtons[i].Disable();
                            }
                        }

                        currentPlayerOffset += playerOffset;
                    }
                }
                // Enable Client to see all players
                else if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
                {
                    List<string> players = MultiplayerManager.Instance.PlayerList.ToList();

                    // List all the players
                    for (int i = 0; i < players.Count; i++)
                    {
                        string player = players[i];

                        if (i < _playerLabels.Count)
                        {
                            _playerLabels[i].text = player;
                            _playerLabels[i].Show();
                            _playerLabels[i].Enable();
                        }
                        else
                        {
                            _playerLabels.Add(this.CreateLabel(player, new Vector2(10, topOffset + currentPlayerOffset)));
                        }

                        currentPlayerOffset += playerOffset;
                    }
                }
            }

            base.Update();
        }
    }
}
