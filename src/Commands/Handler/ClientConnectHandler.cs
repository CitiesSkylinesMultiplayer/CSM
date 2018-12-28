using CSM.Networking;
using CSM.Panels;
using NLog;

namespace CSM.Commands.Handler
{
    public class ClientConnectHandler : CommandHandler<ClientConnectCommand>
    {
        public override byte ID => CommandIds.ClientConnectCommand;

        public override void HandleOnClient(ClientConnectCommand command)
        {
            LogManager.GetCurrentClassLogger().Info($"Player {command.Username} has connected!");
            ChatLogPanel.PrintGameMessage($"Player {command.Username} has connected!");

            MultiplayerManager.Instance.PlayerList.Add(command.Username);
        }

        public override void HandleOnServer(ClientConnectCommand command, Player player)
        {
        }

        public override void OnClientConnect(Player player)
        {
            Command.SendToClients(new ClientConnectCommand { Username = player.Username });
        }
    }
}