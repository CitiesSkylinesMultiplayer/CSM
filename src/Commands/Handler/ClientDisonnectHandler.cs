
using CSM.Networking;

namespace CSM.Commands.Handler
{
    class ClientDisconnectHandler : CommandHandler<ClientDisconnectCommand>
    {
        public override byte ID => 51;

        public override void HandleOnClient(ClientDisconnectCommand command)
        {
            CSM.Log($"Player {command.Username} has disconnected!");
            MultiplayerManager.Instance.PlayerList.Remove(command.Username);
        }

        public override void HandleOnServer(ClientDisconnectCommand command, Player player) { }

        public override void OnClientDisconnect(Player player)
        {
            Command.SendToClients(new ClientDisconnectCommand { Username = player.Username });
        }
    }
}
