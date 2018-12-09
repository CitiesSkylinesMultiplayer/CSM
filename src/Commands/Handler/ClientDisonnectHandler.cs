using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class ClientDisconnectHandler : CommandHandler<ClientDisconnectCommand>
    {
        public override byte ID => CommandIds.ClientDisconnectCommand;

        public override void HandleOnClient(ClientDisconnectCommand command)
        {
            CSM.Log($"Player {command.Username} has disconnected!");
            MultiplayerManager.Instance.PlayerList.Remove(command.Username);
        }

        public override void HandleOnServer(ClientDisconnectCommand command, Player player)
        {
        }

        public override void OnClientDisconnect(Player player)
        {
            Command.SendToClients(new ClientDisconnectCommand { Username = player.Username });
        }
    }
}