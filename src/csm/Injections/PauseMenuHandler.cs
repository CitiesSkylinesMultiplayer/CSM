using ColossalFramework.UI;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.Helpers;
using CSM.Networking;
using CSM.Panels;
using HarmonyLib;
using UnityEngine;

namespace CSM.Injections
{
    [HarmonyPatch(typeof(PauseMenu))]
    [HarmonyPatch("Initialize")]
    public class PauseMenuInitialize
    {
        /// <summary>
        ///     This handler is used to place the 'MULTIPLAYER' button to the top
        ///     of the pause menu.
        /// </summary>
        public static void Prefix()
        {
            PauseMenuHandler.CreateOrUpdateMultiplayerButton();
        }

        public static void Postfix()
        {
            PauseMenuHandler.SetMenuHeight();
        }
    }

    public class PauseMenuHandler
    {
        public static void CreateOrUpdateMultiplayerButton()
        {
            Log.Info("Creating multiplayer button...");

            // Find pause menu view.
            UIPanel pauseUiPanel = UIView.GetAView()?.FindUIComponent("Menu") as UIPanel;

            if (pauseUiPanel == null)
                return;

            // Find button.
            UIButton _multiplayerButton = UIView.GetAView().FindUIComponent("Multiplayer") as UIButton;

            // Find divider.
            UIPanel _multiplayerDivider = UIView.GetAView().FindUIComponent("multiplayerDivider") as UIPanel;

            // Add the button & divider if it does not exist and assign
            // the click event.
            if (_multiplayerButton == null)
            {
                // Add multiplayer button.
                _multiplayerButton = (UIButton)pauseUiPanel.AddUIComponent(typeof(UIButton));

                // Add multiplayer divider.
                _multiplayerDivider = (UIPanel)pauseUiPanel.AddUIComponent(typeof(UIPanel));

                // Respond to button click.
                _multiplayerButton.eventClick += (component, param) =>
                {
                    // Close pause menu.
                    ReflectionHelper.Call(new PauseMenu(), "Resume");

                    // Open host game menu if not in multiplayer session, else open connection panel
                    if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.None)
                    {
                        PanelManager.TogglePanel<HostGamePanel>();

                        // Display warning if DLCs or other mods are enabled
                        if (DLCHelper.GetOwnedExpansions() != SteamHelper.ExpansionBitMask.None || DLCHelper.GetOwnedModderPacks() != SteamHelper.ModderPackBitMask.None)
                        {
                            MessagePanel msgPanel = PanelManager.ShowPanel<MessagePanel>();
                            if (msgPanel)
                                msgPanel.DisplayContentWarning();
                        }
                    }
                    else
                    {
                        PanelManager.TogglePanel<ConnectionPanel>();
                    }
                };
            }

            // Set multiplayer button properties.
            _multiplayerButton.name = "Multiplayer";
            _multiplayerButton.text = "MULTIPLAYER";
            _multiplayerButton.width = 310;
            _multiplayerButton.height = 57;
            _multiplayerButton.textScale = 1.25f;
            _multiplayerButton.normalBgSprite = "ButtonMenu";
            _multiplayerButton.disabledBgSprite = "ButtonMenuDisabled";
            _multiplayerButton.hoveredBgSprite = "ButtonMenuHovered";
            _multiplayerButton.focusedBgSprite = "ButtonMenuFocused";
            _multiplayerButton.pressedBgSprite = "ButtonMenuPressed";
            _multiplayerButton.textColor = new Color32(255, 255, 255, 255);
            _multiplayerButton.disabledTextColor = new Color32(7, 7, 7, 255);
            _multiplayerButton.hoveredTextColor = new Color32(7, 132, 255, 255);
            _multiplayerButton.focusedTextColor = new Color32(255, 255, 255, 255);
            _multiplayerButton.pressedTextColor = new Color32(30, 30, 44, 255);

            // Set multiplayer divider properties.
            _multiplayerDivider.height = 7;
            _multiplayerDivider.name = "multiplayerDivider";

            // Enable button sounds.
            _multiplayerButton.playAudioEvents = true;
        }

        internal static void SetMenuHeight()
        {
            // Find pause menu view.
            UIPanel pauseUiPanel = UIView.GetAView()?.FindUIComponent("Menu") as UIPanel;

            // Set menu height.
            pauseUiPanel.parent.height = 580;
        }
    }
}
