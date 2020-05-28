using System;
using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using CSM.Commands;
using CSM.Commands.Data.Internal;
using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Status;
using CSM.Panels;
using ICities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CSM.Extensions
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private UIButton _multiplayerButton;

        public override void OnReleased()
        {
            // Stop everything
            MultiplayerManager.Instance.StopEverything();

            base.OnReleased();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
            {
                MultiplayerManager.Instance.CurrentClient.Status = ClientStatus.Connected;
                Command.SendToServer(new ClientLevelLoadedCommand());
            }

            UIView uiView = UIView.GetAView();

            // Add the chat log
            uiView.AddUIComponent(typeof(ChatLogPanel));

            _multiplayerButton = (UIButton)uiView.AddUIComponent(typeof(UIButton));

            _multiplayerButton.text = "Multiplayer";
            _multiplayerButton.width = 150;
            _multiplayerButton.height = 40;

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

            // Enable button sounds.
            _multiplayerButton.playAudioEvents = true;

            // Place the button.
            _multiplayerButton.transformPosition = new Vector3(-1.45f, 0.97f);

            // Respond to button click.
            _multiplayerButton.eventClick += (component, param) =>
            {
                // Open host game menu if not in multiplayer session, else open connection panel
                if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.None)
                {
                    PanelManager.TogglePanel<HostGamePanel>();

                    // Display warning if DLCs or other mods are enabled
                    if (DLCHelper.GetOwnedDLCs() != SteamHelper.DLC_BitMask.None ||
                        Singleton<PluginManager>.instance.enabledModCount > 1)
                    {
                        MessagePanel msgPanel = PanelManager.ShowPanel<MessagePanel>();
                        msgPanel.DisplayContentWarning();
                    }
                }
                else
                {
                    PanelManager.TogglePanel<ConnectionPanel>();
                }

                _multiplayerButton.Unfocus();
            };
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            //Code below destroys any created UI from the screen.
            try
            {
                UIComponent _getui = UIView.GetAView().FindUIComponent<UIComponent>("ChatLogPanel");
                UIComponent[] children = _getui.GetComponentsInChildren<UIComponent>();

                foreach (UIComponent child in children)
                {
                    Object.Destroy(child);
                }

                // Destroy duplicated multiplayer button
                UIComponent temp = UIView.GetAView().FindUIComponent("MPConnectionPanel");
                if (temp)
                    Object.Destroy(temp);

                // Destroy multiplayer join panel
                UIComponent clientJoinPanel = UIView.GetAView().FindUIComponent("MPClientJoinPanel");
                if (clientJoinPanel)
                    Object.Destroy(clientJoinPanel);
            }
            catch (NullReferenceException)
            {
                // Ignore, because it sometimes throws them... (Not caused by us)
                // TODO: Rework this to be more stable
            }
        }
    }
}
