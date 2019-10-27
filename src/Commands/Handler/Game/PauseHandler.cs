
using CSM.Commands.Data.Game;

namespace CSM.Commands.Handler.Game
{
    public class PauseHandler : CommandHandler<PauseCommand>
    {
        protected override void Handle(PauseCommand command)
        {
            SimulationManager.instance.SimulationPaused = command.SimulationPaused;
        }
    }
}
