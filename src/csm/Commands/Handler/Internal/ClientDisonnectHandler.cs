using CSM.API;
using CSM.API.Commands;
using CSM.API.Networking;
using CSM.BaseGame.Helpers;
using CSM.Commands.Data.Internal;
using CSM.Helpers;
using CSM.Networking;

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

            if (CSM.IsSteamPresent)
            {
                SteamHelpers.Instance.SetRichPresence("steam_player_group_size", MultiplayerManager.Instance.PlayerList.Count.ToString());
            }
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
