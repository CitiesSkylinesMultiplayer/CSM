using System;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using CSM.API.Commands;
using CSM.API.Networking.Status;
using CSM.Commands;
using CSM.Commands.Data.Internal;
using CSM.Commands.Handler.Game;
using CSM.Helpers;
using CSM.Injections;
using CSM.Mods;
using CSM.Networking;
using CSM.Panels;
using ICities;
using Object = UnityEngine.Object;

namespace CSM.Extensions
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnReleased()
        {
            // Stop everything
            MultiplayerManager.Instance.StopEverything();

            base.OnReleased();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            // When Client loads level then we process events to check if we are timed out while loading (not Client
            // anymore). After that we return, because Client disconnect unloads level.
            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
            {
                Singleton<SimulationManager>.instance.m_ThreadingWrapper.QueueSimulationThread(() =>
                {
                    MultiplayerManager.Instance.ProcessEvents();
                    if (MultiplayerManager.Instance.CurrentRole != MultiplayerRole.Client)
                    {
                        return;
                    }

                    MultiplayerManager.Instance.CurrentClient.Status = ClientStatus.Connected;
                    CommandInternal.Instance.SendToServer(new ClientLevelLoadedCommand());
                });
            }

            ModSupport.Instance.OnLevelLoaded(mode);

            ResetData();

            // Add the chat log
            UIView.GetAView().AddUIComponent(typeof(ChatLogPanel));

            // Setup Pause menu.
            PauseMenuHandler.CreateOrUpdateMultiplayerButton();

            // Show release notes if not shown for this version
            Version version = Assembly.GetAssembly(typeof(CSM)).GetName().Version;
            string strVersion = $"{version.Major}.{version.Minor}";
            if (string.Compare(strVersion, CSM.Settings.LastSeenReleaseNotes) > 0)
            {
                MessagePanel panel = PanelManager.ShowPanel<MessagePanel>();
                if (panel)
                {
                    panel.DisplayReleaseNotes();
                    CSM.Settings.LastSeenReleaseNotes.value = strVersion;
                }
            }
        }

        private void ResetData()
        {
            // Pause simulation on game load and reset cached speed and pause states
            SpeedPauseHelper.ResetSpeedAndPauseState();
            
            // Reset waiting id as the last speed/pause request is no longer valid
            SpeedPauseResponseHandler.ResetWaitingId();
            
            // Don't handle dropped frames from before the map was loaded
            SlowdownHelper.ClearDropFrames();
            SlowdownHelper.ClearLocalDropFrames();

            // TODO: Check if we need to reset more caches
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            ModSupport.Instance.OnLevelUnloading();

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
