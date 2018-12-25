using CSM.Commands.Data;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class FinishTransactionHandler : CommandHandler<FinishTransactionCommand>
    {
        public override byte ID => CommandIds.FinishTransactionCommand;

        public override void HandleOnServer(FinishTransactionCommand command, Player player)
        {
            TransactionHandler.FinishReceived(command.Type, player);
        }

        public override void HandleOnClient(FinishTransactionCommand command)
        {
            TransactionHandler.FinishReceived(command.Type, null);
        }
    }
}