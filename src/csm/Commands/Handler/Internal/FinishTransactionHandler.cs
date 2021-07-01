using CSM.API.Commands;
using CSM.Commands.Data.Internal;

namespace CSM.Commands.Handler.Internal
{
    public class FinishTransactionHandler : CommandHandler<FinishTransactionCommand>
    {
        public FinishTransactionHandler()
        {
            TransactionCmd = false;
        }

        protected override void Handle(FinishTransactionCommand command)
        {
            TransactionHandler.FinishReceived(command.SenderId);
        }
    }
}
