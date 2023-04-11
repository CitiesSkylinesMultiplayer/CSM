using CSM.API;
using CSM.API.Commands;
using CSM.API.Networking;
using CSM.BaseGame.Helpers;
using CSM.Commands.Data.Internal;
using CSM.Helpers;
using CSM.Networking;
using ColossalFramework;
using CSM.BaseGame.Injections.Tools;

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
            Singleton<ToolSimulator>.instance.RemoveSender(command.ClientId);
            Singleton<ToolSimulatorCursorManager>.instance.RemoveCursorView(command.ClientId);

            if (CSM.IsSteamPresent)
            {
                SteamHelpers.Instance.SetGroupSize(MultiplayerManager.Instance.PlayerList.Count);
            }
        }

        public override void OnClientDisconnect(Player player)
        {
            int clientId = player is CSMPlayer csmPlayer ? csmPlayer.NetPeer.Id : -2;
            Command.SendToClients(new ClientDisconnectCommand
            {
                Username = player.Username,
                ClientId = clientId
            });
        }
    }
}
