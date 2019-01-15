using CSM.Commands.Data;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class FinishTransactionHandler : CommandHandler<FinishTransactionCommand>
    {
        public override byte ID => CommandIds.FinishTransactionCommand;

        public FinishTransactionHandler()
        {
            RelayOnServer = false;
            TransactionCmd = false;
        }

        public override void HandleOnServer(FinishTransactionCommand command, Player player)
        {
            TransactionHandler.FinishReceived(player);
        }

        public override void HandleOnClient(FinishTransactionCommand command)
        {
            TransactionHandler.FinishReceived(null);
        }
    }
}