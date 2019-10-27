
using CSM.Commands.Data.Game;

namespace CSM.Commands.Handler.Game
{
    public class SpeedHandler : CommandHandler<SpeedCommand>
    {
        protected override void Handle(SpeedCommand command)
        {
            SimulationManager.instance.SelectedSimulationSpeed = command.SelectedSimulationSpeed;
        }
    }
}
