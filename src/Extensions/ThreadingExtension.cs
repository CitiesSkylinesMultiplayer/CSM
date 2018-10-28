using CSM.Commands;
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

        public override void OnAfterSimulationTick()
        {
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
        }
    }
}