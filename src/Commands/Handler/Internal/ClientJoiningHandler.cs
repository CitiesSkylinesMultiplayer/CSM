using CSM.Commands.Data.Internal;
using CSM.Networking;
using CSM.Networking.Status;

namespace CSM.Commands.Handler.Internal
{
    class ClientJoiningHandler : CommandHandler<ClientJoiningCommand>
    {
        public ClientJoiningHandler()
        {
            TransactionCmd = false;
        }

        protected override void Handle(ClientJoiningCommand command)
        {
            if (command.JoiningFinished)
            {
                MultiplayerManager.Instance.UnblockGame();
            }
            else
            {
                MultiplayerManager.Instance.BlockGame(command.JoiningUsername);
            }
        }

        public override void OnClientDisconnect(Player player)
        {
            if (player.Status != ClientStatus.Connected)
            {
                Command.SendToClients(new ClientJoiningCommand
                {
                    JoiningFinished = true,
                    JoiningUsername = player.Username
                });
                MultiplayerManager.Instance.UnblockGame();
            }
        }
    }
}
