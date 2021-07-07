using CSM.Commands.Data.Internal;
using CSM.Networking;
using CSM.Networking.Status;

namespace CSM.Commands.Handler.Internal
{
    public class ClientLevelLoadedHandler : CommandHandler<ClientLevelLoadedCommand>
    {
        public ClientLevelLoadedHandler()
        {
            RelayOnServer = false;
            TransactionCmd = false;
        }

        protected override void Handle(ClientLevelLoadedCommand command)
        {
            Player P = MultiplayerManager.Instance.CurrentServer.ConnectedPlayers[command.SenderId];
            P.Status = ClientStatus.Connected;
            Command.SendToOtherClients(new ClientJoiningCommand
            {
                JoiningFinished = true,
                JoiningUsername = P.Username
            }, P);
            MultiplayerManager.Instance.UnblockGame();
        }
    }
}
