using CSM.API.Commands;
using CSM.API.Networking;
using CSM.API.Networking.Status;
using CSM.Commands.Data.Internal;
using CSM.Networking;

namespace CSM.Commands.Handler.Internal
{
    public class ClientJoiningHandler : CommandHandler<ClientJoiningCommand>
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

        public override void OnClientConnect(global::CSM.API.Networking.Player player)
        {
            var internalPlayer = player as global::CSM.Networking.Player;
            if (internalPlayer != null && internalPlayer.Status != global::CSM.API.Networking.Status.ClientStatus.Connected)
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
