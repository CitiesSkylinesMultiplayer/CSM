using System;
using CSM.API.Commands;
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

            // Process events of the network lib
            MultiplayerManager.Instance.ProcessEvents();
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
        }
    }
}
