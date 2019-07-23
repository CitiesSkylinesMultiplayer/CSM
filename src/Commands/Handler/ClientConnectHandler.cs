using CSM.Networking;
using CSM.Panels;
using NLog;
using CSM.Localisation;

namespace CSM.Commands.Handler
{
    public class ClientConnectHandler : CommandHandler<ClientConnectCommand>
    {
        public ClientConnectHandler()
        {
            TransactionCmd = false;
        }

        public override void Handle(ClientConnectCommand command)
        {
            LogManager.GetCurrentClassLogger().Info($"Player {command.Username} has connected!");
            ChatLogPanel.PrintGameMessage($"{Translation.PullTranslation("Player")} {command.Username} {Translation.PullTranslation("HasConnected", true)}");

            MultiplayerManager.Instance.PlayerList.Add(command.Username);
        }

        public override void OnClientConnect(Player player)
        {
            Command.SendToClients(new ClientConnectCommand { Username = player.Username });
        }
    }
}
