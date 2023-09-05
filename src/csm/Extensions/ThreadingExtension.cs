using System;
using ColossalFramework;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.API.Networking.Status;
using CSM.BaseGame.Injections;
using CSM.Commands;
using CSM.Helpers;
using CSM.Networking;
using ICities;

namespace CSM.Extensions
{
    public class ThreadingExtension : ThreadingExtensionBase
    {
        private DateTime _lastEconomyAndDropSync;

        public override void OnCreated(IThreading threading)
        {
            Singleton<MainThreadTracker>.Ensure();
        }

        public override void OnBeforeSimulationTick()
        {
            base.OnBeforeSimulationTick();

            // Normally, the game is paused when the player is in Esc or similar menus. We ignore this setting.
            if (SimulationManager.instance.ForcedSimulationPaused && MultiplayerManager.Instance.CurrentRole != MultiplayerRole.None)
            {
                SimulationManager.instance.ForcedSimulationPaused = false;
            }

            // Process changes in the pause state and game speed
            SpeedPauseHelper.SimulationStep();

            // Don't process events while the client is loading the level.
            // It first needs to be processed in loading extension to make sure the level is fully loaded.
            if (MultiplayerManager.Instance.CurrentRole != MultiplayerRole.Client ||
                (MultiplayerManager.Instance.CurrentClient.Status == ClientStatus.Connected ||
                 MultiplayerManager.Instance.CurrentClient.Status == ClientStatus.Downloading))
            {
                // Process events of the network lib
                MultiplayerManager.Instance.ProcessEvents();
            }
        }

        public override void OnAfterSimulationTick()
        {
            // Send economy and frame drop packets every two seconds
            if (DateTime.Now.Subtract(_lastEconomyAndDropSync).TotalSeconds > 2)
            {
                // Only send economy and dropped frames when connected
                // (loading may accumulate dropped frames we need to ignore)
                if (MultiplayerManager.Instance.IsConnected())
                {
                    ResourceCommandHandler.Send();
                    SlowdownHelper.SendDroppedFrames();
                }
                _lastEconomyAndDropSync = DateTime.Now;
            }

            // Finish transactions
            TransactionHandler.FinishSend();

            // Check if ignore helper is still in ignore state at the end of the simulation step
            if (IgnoreHelper.Instance.IsIgnored())
            {
                Log.Warn("Ignore helper not stopped at end of simulation tick. A restart may be required.");
                Chat.Instance.PrintGameMessage(Chat.MessageType.Warning, "Warning: Synchronization problem detected. If you encounter problems, restart the multiplayer session!");
                IgnoreHelper.Instance.ResetIgnore();
            }
        }
    }

    class MainThreadTracker : Singleton<MainThreadTracker>
    {
        public void LateUpdate()
        {
            // Check if ignore helper is in ignore state outside of other Update functions
            if (IgnoreHelper.Instance.IsIgnored())
            {
                Log.Warn("Ignore helper not stopped at arbitrary point of UI tick. A restart may be required.");
                Chat.Instance.PrintGameMessage(Chat.MessageType.Warning, "Warning: Synchronization problem detected. If you encounter problems, restart the multiplayer session!");
                IgnoreHelper.Instance.ResetIgnore();
            }
        }
    }
}
