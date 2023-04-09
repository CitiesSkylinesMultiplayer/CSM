using CSM.API.Commands;
using CSM.API.Networking;
using CSM.Commands.Data.Internal;
using CSM.Helpers;
using CSM.Networking;

namespace CSM.Commands.Handler.Internal
{
    public class PlayerListHandler : CommandHandler<PlayerListCommand>
    {
        protected override void Handle(PlayerListCommand command)
        {
            MultiplayerManager.Instance.PlayerList.Clear();
            MultiplayerManager.Instance.PlayerList.UnionWith(command.PlayerList);

            if (CSM.IsSteamPresent)
            {
                SteamHelpers.Instance.SetGroupSize(MultiplayerManager.Instance.PlayerList.Count);
            }
        }

        public override void OnClientConnect(Player player)
        {
            // Send current player list
            CommandInternal.Instance.SendToClient(player, new PlayerListCommand { PlayerList = MultiplayerManager.Instance.PlayerList });
        }
    }
}
