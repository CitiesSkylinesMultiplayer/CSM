using CSM.API.Commands;
using CSM.Commands.Data.Game;
using CSM.Helpers;
using CSM.Networking;

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
            // If we are not yet connected don't answer as we're not counted towards
            // the "number of required consenting clients" until we are done with loading
            if (!MultiplayerManager.Instance.IsConnected())
            {
                return;
            }

            SpeedPauseHelper.PlayPauseRequest(command.SimulationPaused, command.SelectedSimulationSpeed, command.RequestId);
        }
    }
}
