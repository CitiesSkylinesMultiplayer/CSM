using CSM.API;
using CSM.API.Commands;
using CSM.API.Networking;
using CSM.BaseGame.Helpers;
using CSM.Commands.Data.Internal;
using CSM.Networking;
using CSM.Util;

namespace CSM.Commands.Handler.Internal
{
    public class ClientDisconnectHandler : CommandHandler<ClientDisconnectCommand>
    {
        public ClientDisconnectHandler()
        {
            TransactionCmd = false;
        }

        protected override void Handle(ClientDisconnectCommand command)
        {
            Log.Info($"Player {command.Username} has disconnected!");
            Chat.Instance.PrintGameMessage($"Player {command.Username} has disconnected!");

            MultiplayerManager.Instance.PlayerList.Remove(command.Username);

            TransactionHandler.ClearTransactions(command.ClientId);
            ToolSimulator.RemoveSender(command.ClientId);
        }

        public override void OnClientDisconnect(Player player)
        {
            Command.SendToClients(new ClientDisconnectCommand
            {
                Username = player.Username,
                ClientId = player.NetPeer.Id
            });
        }
    }
}
