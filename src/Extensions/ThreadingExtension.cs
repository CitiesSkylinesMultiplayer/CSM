using CSM.Commands;
using CSM.Commands.Data.Game;
using CSM.Networking;
using ICities;

namespace CSM.Extensions
{
    /// <summary>
    ///     TODO, We need to sync the current time as well
    /// </summary>
    public class ThreadingExtension : ThreadingExtensionBase
    {
        private int _lastSelectedSimulationSpeed;
        private bool _lastSimulationPausedState;

        public override void OnBeforeSimulationTick()
        {
            base.OnBeforeSimulationTick();
            MultiplayerManager.Instance.ProcessEvents();
        }

        public override void OnAfterSimulationTick()
        {
            // Normally, the game is paused when the player is in Esc or similar menus. We ignore this setting.
            if (SimulationManager.instance.ForcedSimulationPaused && MultiplayerManager.Instance.CurrentRole != MultiplayerRole.None)
            {
                SimulationManager.instance.ForcedSimulationPaused = false;
            }

            if (_lastSelectedSimulationSpeed != SimulationManager.instance.SelectedSimulationSpeed)
            {
                Command.SendToAll(new SpeedCommand
                {
                    SelectedSimulationSpeed = SimulationManager.instance.SelectedSimulationSpeed
                });
            }

            if (_lastSimulationPausedState != SimulationManager.instance.SimulationPaused)
            {
                Command.SendToAll(new PauseCommand
                {
                    SimulationPaused = SimulationManager.instance.SimulationPaused
                });
            }

            _lastSimulationPausedState = SimulationManager.instance.SimulationPaused;
            _lastSelectedSimulationSpeed = SimulationManager.instance.SelectedSimulationSpeed;

            // Finish transactions
            TransactionHandler.FinishSend();
        }
    }
}
