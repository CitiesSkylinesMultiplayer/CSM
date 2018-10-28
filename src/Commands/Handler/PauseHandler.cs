
using CSM.Networking;

namespace CSM.Commands.Handler
{
    class PauseHandler : CommandHandler<PauseCommand>
    {
        public override byte ID => 101;

        public override void HandleOnClient(PauseCommand command) => Handle(command);

        public override void HandleOnServer(PauseCommand command, Player player) => Handle(command);

        private void Handle(PauseCommand command)
        {
            SimulationManager.instance.SimulationPaused = command.SimulationPaused;
        }
    }
}
