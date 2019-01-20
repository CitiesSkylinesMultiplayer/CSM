
namespace CSM.Commands.Handler
{
    public class SpeedHandler : CommandHandler<SpeedCommand>
    {
        public override void Handle(SpeedCommand command)
        {
            SimulationManager.instance.SelectedSimulationSpeed = command.SelectedSimulationSpeed;
        }
    }
}
