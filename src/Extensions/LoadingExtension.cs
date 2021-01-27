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
using System;
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

            UIButton resumeButton = UIView.GetAView()?.FindUIComponent("Resume") as UIButton;

            // Check if resume button exists.
            if (resumeButton == null)
            {
                return;
            }

            // Set btn text.
            resumeButton.text = "HOST GAME";
            
            // Add additional eventClick.
            resumeButton.eventClick += (s, e) =>
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
            };

            UIView uiView = UIView.GetAView();

            // Add the chat log
            uiView.AddUIComponent(typeof(ChatLogPanel));
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
