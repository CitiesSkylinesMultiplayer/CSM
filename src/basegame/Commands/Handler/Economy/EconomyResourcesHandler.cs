using CSM.API.Commands;
using CSM.BaseGame.Commands.Data.Economy;

namespace CSM.BaseGame.Commands.Handler.Economy
{
    public class EconomyResourcesHandler : CommandHandler<EconomyResourcesCommand>
    {
        public EconomyResourcesHandler()
        {
            TransactionCmd = false;
        }

        protected override void Handle(EconomyResourcesCommand command)
        {
            foreach (EconomyResourceCommand cmd in command.Commands)
            {
                Command.GetCommandHandler(cmd.GetType()).Parse(cmd);
            }
        }
    }
}
