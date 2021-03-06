using CSM.Commands.Data.Internal;
using CSM.Networking;

namespace CSM.Commands.Handler.Internal
{
    public class RequestWorldTransferHandler : CommandHandler<RequestWorldTransferCommand>
    {
        public RequestWorldTransferHandler()
        {
            TransactionCmd = false;
            RelayOnServer = false;
        }

        protected override void Handle(RequestWorldTransferCommand command)
        {
            Player newPlayer = MultiplayerManager.Instance.CurrentServer.ConnectedPlayers[command.SenderId];
            ConnectionRequestHandler.PrepareWorldLoad(newPlayer);
        }
    }
}
