using System.Runtime.Remoting.Messaging;
using CSM.Commands;
using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Status;
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
            if (_lastSimulationPausedState != SimulationManager.instance.SimulationPaused ||
                _lastSelectedSimulationSpeed != SimulationManager.instance.SelectedSimulationSpeed)
            {


                switch (MultiplayerManager.Instance.CurrentRole)
                {
                    case MultiplayerRole.Client:
                        if (MultiplayerManager.Instance.CurrentClient.Status == ClientStatus.Connected)
                        {
                            MultiplayerManager.Instance.CurrentClient.NetClient.GetFirstPeer().Send(ArrayHelpers.PrependByte(CommandBase.SimulationCommandID, new SimulationCommand
                            {
                                SumulationSpeed = SimulationManager.instance.SelectedSimulationSpeed,
                                SimulationPaused = SimulationManager.instance.SimulationPaused
                            }.Serialize()), SendOptions.ReliableOrdered);
                        }

                        break;
                    case MultiplayerRole.Server:
                            MultiplayerManager.Instance.CurrentServer.SendToClients(CommandBase.SimulationCommandID, new SimulationCommand
                            {
                                SumulationSpeed = SimulationManager.instance.SelectedSimulationSpeed,
                                SimulationPaused = SimulationManager.instance.SimulationPaused
                            });
                        break;
                }
            }

            _lastSelectedSimulationSpeed = SimulationManager.instance.SelectedSimulationSpeed;
            _lastSimulationPausedState = SimulationManager.instance.SimulationPaused;

            base.OnAfterSimulationTick();
        }
    }
}
