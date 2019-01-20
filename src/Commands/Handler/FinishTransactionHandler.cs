using CSM.Commands.Data;

namespace CSM.Commands.Handler
{
    public class FinishTransactionHandler : CommandHandler<FinishTransactionCommand>
    {
        public FinishTransactionHandler()
        {
            TransactionCmd = false;
        }

        public override void Handle(FinishTransactionCommand command)
        {
            TransactionHandler.FinishReceived(command.SenderId);
        }
    }
}
