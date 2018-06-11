using CSM.Commands;
using CSM.Helpers;
using CSM.Networking;
using ICities;
using LiteNetLib;

namespace CSM.Extensions
{
    public class ThreadingExtension : ThreadingExtensionBase
    {
        private int _lastSelectedSimulationSpeed;
        private bool _lastSimulationPausedState;


        public override void OnAfterSimulationTick()
        {
            if (_lastSimulationPausedState != SimulationManager.instance.ForcedSimulationPaused ||
                _lastSelectedSimulationSpeed != SimulationManager.instance.SelectedSimulationSpeed)
            {
                switch (MultiplayerManager.Instance.CurrentRole)
                {
                    case MultiplayerRole.Client:
                        MultiplayerManager.Instance.CurrentClient.NetClient.GetFirstPeer().Send(ArrayHelpers.PrependByte(CommandBase.SimulationCommandID, new SimulationCommand
                        {
                            SumulationSpeed = SimulationManager.instance.SelectedSimulationSpeed,
                            SimulationPaused = SimulationManager.instance.SimulationPaused
                        }.Serialize()), SendOptions.ReliableOrdered);
                        break;
                    case MultiplayerRole.Server:
                        MultiplayerManager.Instance.CurrentServer.NetServer.SendToAll(ArrayHelpers.PrependByte(CommandBase.SimulationCommandID, new SimulationCommand
                        {
                            SumulationSpeed = SimulationManager.instance.SelectedSimulationSpeed,
                            SimulationPaused = SimulationManager.instance.SimulationPaused
                        }.Serialize()), SendOptions.ReliableOrdered);
                        break;
                }
            }

            _lastSelectedSimulationSpeed = SimulationManager.instance.SelectedSimulationSpeed;
            _lastSimulationPausedState = SimulationManager.instance.SimulationPaused;

            base.OnAfterSimulationTick();
        }
    }
}
