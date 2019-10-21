namespace CSM.Commands.Handler
{
    public class PauseHandler : CommandHandler<PauseCommand>
    {
        public override void Handle(PauseCommand command)
        {
            SimulationManager.instance.SimulationPaused = command.SimulationPaused;
        }
    }
}
