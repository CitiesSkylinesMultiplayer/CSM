﻿using ColossalFramework.UI;
using CSM.Helpers;
using CSM.Networking;
using UnityEngine;

namespace CSM.Panels
{
    public class ConnectionPanel : UIPanel
    {
        private UIButton _disconnectButton;

        private UIButton _serverManageButton;

        private UICheckBox _playerPointers;
        public static bool showPlayerPointers = false;

        public override void Start()
        {
            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            color = new Color32(110, 110, 110, 250);

            // Grab the view for calculating width and height of game
            UIView view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(view.fixedWidth / 2.0f - 180.0f, view.fixedHeight / 2.0f - 100.0f);

            width = 360;
            height = 240;

            // Handle visible change events
            eventVisibilityChanged += (component, visible) =>
            {
                if (!visible)
                    return;

                RefreshState();
            };

            this.CreateTitleLabel("Multiplayer Menu", new Vector3(80, -20, 0));

            // Manage server button
            _serverManageButton = this.CreateButton("Manage Server", new Vector2(10, -60));
            Hide(_serverManageButton);

            // Close server button
            _disconnectButton = this.CreateButton("Stop Server", new Vector2(10, -130));

            // Show Player Pointers
            _playerPointers = this.CreateCheckBox("Show Player Pointers", new Vector2(10, -210));
            _playerPointers.eventClicked += (component, param) =>
            {
                showPlayerPointers = _playerPointers.isChecked;
            };

            _disconnectButton.eventClick += (component, param) =>
            {
                isVisible = false;
                MultiplayerManager.Instance.StopEverything();
            };

            _serverManageButton.eventClick += (component, param) =>
            {
                PanelManager.ShowPanel<ManageGamePanel>();

                isVisible = false;
            };

            base.Start();

            RefreshState();
        }

        /// <summary>
        ///     Refresh which buttons should be displayed on the user interface
        /// </summary>
        public void RefreshState()
        {
            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
            {
                Show(_serverManageButton);

                _disconnectButton.text = "Stop server";
            }
            else if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
            {
                Hide(_serverManageButton);

                _disconnectButton.text = "Disconnect";
            }
        }

        private void Show(UIButton button)
        {
            button.isVisible = true;
            button.isEnabled = true;
        }

        private void Hide(UIButton button)
        {
            button.isVisible = false;
            button.isEnabled = false;
        }
    }
}
