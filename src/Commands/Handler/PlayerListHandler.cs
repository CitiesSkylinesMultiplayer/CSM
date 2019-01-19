using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class PlayerListHandler : CommandHandler<PlayerListCommand>
    {
        public override void Handle(PlayerListCommand command)
        {
            MultiplayerManager.Instance.PlayerList.Clear();
            MultiplayerManager.Instance.PlayerList.UnionWith(command.PlayerList);
        }

        public override void OnClientConnect(Player player)
        {
            // Send current player list
            Command.SendToClient(player, new PlayerListCommand { PlayerList = MultiplayerManager.Instance.PlayerList });
        }
    }
}
