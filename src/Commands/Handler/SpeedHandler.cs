using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class SpeedHandler : CommandHandler<SpeedCommand>
    {
        public override byte ID => 100;

        public override void HandleOnClient(SpeedCommand command) => Handle(command);

        public override void HandleOnServer(SpeedCommand command, Player player) => Handle(command);

        private void Handle(SpeedCommand command)
        {
            SimulationManager.instance.SelectedSimulationSpeed = command.SelectedSimulationSpeed;
        }
    }
}