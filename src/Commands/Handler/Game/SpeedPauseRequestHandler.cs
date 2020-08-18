using CSM.Commands.Data.Game;
using CSM.Helpers;

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
            SpeedPauseHelper.PlayPauseRequest(command.SimulationPaused, command.SelectedSimulationSpeed, command.RequestId);
        }
    }
}
