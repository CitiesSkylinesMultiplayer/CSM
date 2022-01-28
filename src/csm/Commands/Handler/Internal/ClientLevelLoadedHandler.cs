using CSM.API.Commands;
using CSM.API.Networking;
using CSM.API.Networking.Status;
using CSM.Commands.Data.Internal;
using CSM.Networking;

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
            CommandInternal.Instance.SendToOtherClients(new ClientJoiningCommand
            {
                JoiningFinished = true,
                JoiningUsername = P.Username
            }, P);
            MultiplayerManager.Instance.UnblockGame();
        }
    }
}
