using CSM.Common;
using CSM.Networking;
using CSM.Panels;
using NLog;

namespace CSM.Commands.Handler
{
    public class ClientDisconnectHandler : CommandHandler<ClientDisconnectCommand>
    {
        public ClientDisconnectHandler()
        {
            TransactionCmd = false;
        }

        public override void Handle(ClientDisconnectCommand command)
        {
            LogManager.GetCurrentClassLogger().Info($"Player {command.Username} has disconnected!");
            ChatLogPanel.PrintGameMessage($"Player {command.Username} has disconnected!");

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
