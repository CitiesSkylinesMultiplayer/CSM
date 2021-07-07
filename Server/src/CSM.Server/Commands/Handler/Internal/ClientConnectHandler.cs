using CSM.Commands.Data.Internal;
using CSM.Networking;
using CSM.Server.Util;

namespace CSM.Commands.Handler.Internal
{
    public class ClientConnectHandler : CommandHandler<ClientConnectCommand>
    {
        public ClientConnectHandler()
        {
            TransactionCmd = false;
        }

        protected override void Handle(ClientConnectCommand command)
        {
            Log.Info($"Player {command.Username} has connected!");
            ChatLogPanel.PrintGameMessage($"Player {command.Username} has connected!");

            MultiplayerManager.Instance.PlayerList.Add(command.Username);
        }

        public override void OnClientConnect(Player player)
        {
            Command.SendToClients(new ClientConnectCommand { Username = player.Username });
        }
    }
}
