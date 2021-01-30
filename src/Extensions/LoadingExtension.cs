using ColossalFramework.UI;
using CSM.Commands;
using CSM.Commands.Data.Internal;
using CSM.Networking;
using CSM.Networking.Status;
using CSM.Panels;
using CSM.Injections;
using ICities;
using System;
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

            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
            {
                MultiplayerManager.Instance.CurrentClient.Status = ClientStatus.Connected;
                Command.SendToServer(new ClientLevelLoadedCommand());
            }

            // Add the chat log
            UIView.GetAView().AddUIComponent(typeof(ChatLogPanel));

            // Setup Pause menu.
            PauseMenuHandler.CreateOrUpdateMultiplayerButton();
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
