using CSM.Commands.Data.Game;
using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Status;

namespace CSM.Commands.Handler.Game
{
    public class SpeedPauseRequestHandler : CommandHandler<SpeedPauseRequestCommand>
    {
        public SpeedPauseRequestHandler()
        {
            TransactionCmd = false;
        }

        protected override void Handle(SpeedPauseRequestCommand command)
        {
            // If we are a client but not yet connected, don't answer as we're not counted towards
            // the "number of required consenting clients" until we are done with loading
            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client &&
                MultiplayerManager.Instance.CurrentClient.Status != ClientStatus.Connected)
            {
                return;
            }

            SpeedPauseHelper.PlayPauseRequest(command.SimulationPaused, command.SelectedSimulationSpeed, command.RequestId);
        }
    }
}
