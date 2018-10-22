using CSM.Commands;
using CSM.Networking;
using ICities;
using UnityEngine;

namespace CSM.Extensions
{
    /// <summary>
    /// </summary>
    public class ThreadingExtension : ThreadingExtensionBase
    {


        private int _lastSelectedSimulationSpeed;
        private bool _lastSimulationPausedState;

        public override void OnAfterSimulationTick()
        {
            if (_lastSelectedSimulationSpeed != SimulationManager.instance.SelectedSimulationSpeed)
            {
                switch (MultiplayerManager.Instance.CurrentRole)
                {
                    case MultiplayerRole.Client:
                        MultiplayerManager.Instance.CurrentClient.SendToServer(CommandBase.SpeedCommandID, new SpeedCommand
                        {
                            SelectedSimulationSpeed = SimulationManager.instance.SelectedSimulationSpeed
                        });
                        break;

                    case MultiplayerRole.Server:
                        MultiplayerManager.Instance.CurrentServer.SendToClients(CommandBase.SpeedCommandID, new SpeedCommand
                        {
                            SelectedSimulationSpeed = SimulationManager.instance.SelectedSimulationSpeed
                        });
                        break;
                }
            }
            if (_lastSimulationPausedState != SimulationManager.instance.SimulationPaused)
            {
                switch (MultiplayerManager.Instance.CurrentRole)
                {
                    case MultiplayerRole.Client:
                        MultiplayerManager.Instance.CurrentClient.SendToServer(CommandBase.PauseCommandID, new PauseCommand
                        {
                            SimulationPaused = SimulationManager.instance.SimulationPaused
                        });
                        break;

                    case MultiplayerRole.Server:
                        MultiplayerManager.Instance.CurrentServer.SendToClients(CommandBase.PauseCommandID, new PauseCommand
                        {
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