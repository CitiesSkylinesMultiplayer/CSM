using CSM.API;
using CSM.API.Commands;
using CSM.API.Networking;
using CSM.Commands.Data.Internal;
using CSM.Helpers;
using CSM.Networking;

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
            // When connecting, chat is not initialized yet so we need to ignore this first message
            if (Chat.Instance != null)
            {
                Chat.Instance.PrintGameMessage($"Player {command.Username} has connected!");
            }

            MultiplayerManager.Instance.PlayerList.Add(command.Username);
            if (CSM.IsSteamPresent)
            {
                SteamHelpers.Instance.SetGroupSize(MultiplayerManager.Instance.PlayerList.Count);
            }
        }

        public override void OnClientConnect(Player player)
        {
            CommandInternal.Instance.SendToOtherClients(new ClientConnectCommand { Username = player.Username }, player);
        }
    }
}
