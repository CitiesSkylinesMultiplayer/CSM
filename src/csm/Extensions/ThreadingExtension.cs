using CSM.Commands;
using CSM.Networking;
using ICities;

namespace CSM.Extensions
{
    /// <summary>
    ///     TODO: Pausing simulation does not work.
    /// </summary>
    public class ThreadingExtension : ThreadingExtensionBase
    {
        private int _lastSelectedSimulationSpeed;
        private bool _lastSimulationPausedState;

        public override void OnAfterSimulationTick()
        {
            if (_lastSimulationPausedState != SimulationManager.instance.SimulationPaused ||
                    _lastSelectedSimulationSpeed != SimulationManager.instance.SelectedSimulationSpeed)
            {
                switch (MultiplayerManager.Instance.CurrentRole)
                {
                    case MultiplayerRole.Client:
                        MultiplayerManager.Instance.CurrentClient.SendToServer(CommandBase.SimulationCommandID, new SimulationCommand
                        {
                            SelectedSimulationSpeed = SimulationManager.instance.SelectedSimulationSpeed,
                            SimulationPaused = SimulationManager.instance.SimulationPaused
                        });
                        break;
                    case MultiplayerRole.Server:
                        MultiplayerManager.Instance.CurrentServer.SendToClients(CommandBase.SimulationCommandID, new SimulationCommand
                        {
                            SelectedSimulationSpeed = SimulationManager.instance.SelectedSimulationSpeed,
                            SimulationPaused = SimulationManager.instance.SimulationPaused
                        });
                        break;
                }
            }

            _lastSimulationPausedState = SimulationManager.instance.SimulationPaused;
            _lastSelectedSimulationSpeed = SimulationManager.instance.SelectedSimulationSpeed;
        }
    }
}