using CSM.API.Commands;
using CSM.API.Networking;
using CSM.Commands.Data.Internal;
using CSM.Networking;

namespace CSM.Commands.Handler.Internal
{
    public class PlayerListHandler : CommandHandler<PlayerListCommand>
    {
        protected override void Handle(PlayerListCommand command)
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
