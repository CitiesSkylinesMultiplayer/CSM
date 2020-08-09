using CSM.Commands;
using CSM.Injections;
using CSM.Networking;
using ICities;
using System;
using CSM.Helpers;
using NLog;

namespace CSM.Extensions
{
    public class ThreadingExtension : ThreadingExtensionBase
    {
        private int _lastSelectedSimulationSpeed = -1;
        private bool _lastSimulationPausedState;
        private DateTime _lastEconomyAndDropSync;

        public override void OnBeforeSimulationTick()
        {
            base.OnBeforeSimulationTick();

            // Make sure the games stays paused when blocked (e.g. when another player is joining)
            if (_lastSimulationPausedState != SimulationManager.instance.SimulationPaused
                && MultiplayerManager.Instance.GameBlocked)
            {
                SimulationManager.instance.SimulationPaused = true;
            }
            
            // Normally, the game is paused when the player is in Esc or similar menus. We ignore this setting.
            if (SimulationManager.instance.ForcedSimulationPaused && MultiplayerManager.Instance.CurrentRole != MultiplayerRole.None)
            {
                SimulationManager.instance.ForcedSimulationPaused = false;
            }

            // First tick in the game, initialize speed and pause state tracking variables
            if (_lastSelectedSimulationSpeed == -1)
            {
                _lastSelectedSimulationSpeed = SimulationManager.instance.SelectedSimulationSpeed;
                _lastSimulationPausedState = SimulationManager.instance.SimulationPaused;
                SpeedPauseHelper.Initialize(_lastSimulationPausedState, _lastSelectedSimulationSpeed);
            }

            // Check if speed or pause state have changed
            if (_lastSelectedSimulationSpeed != SimulationManager.instance.SelectedSimulationSpeed ||
                _lastSimulationPausedState != SimulationManager.instance.SimulationPaused)
            {
                SpeedPauseHelper.PlayPauseSpeedChanged(SimulationManager.instance.SimulationPaused,
                    SimulationManager.instance.SelectedSimulationSpeed);

                // Temporarily reset to old value while waiting for a changed state
                SimulationManager.instance.SimulationPaused = _lastSimulationPausedState;
                SimulationManager.instance.SelectedSimulationSpeed = _lastSelectedSimulationSpeed;
            }

            // Update play/pause state if needed
            SpeedPauseHelper.ChangePauseStateIfNeeded();

            _lastSimulationPausedState = SimulationManager.instance.SimulationPaused;
            _lastSelectedSimulationSpeed = SimulationManager.instance.SelectedSimulationSpeed;

            MultiplayerManager.Instance.ProcessEvents();
        }

        public override void OnAfterSimulationTick()
        {
            // Send economy packets every two seconds
            if (DateTime.Now.Subtract(_lastEconomyAndDropSync).TotalSeconds > 2)
            {
                ResourceCommandHandler.Send();
                SlowdownHelper.SendDroppedFrames();
                _lastEconomyAndDropSync = DateTime.Now;
            }

            // Finish transactions
            TransactionHandler.FinishSend();
        }
    }
}
