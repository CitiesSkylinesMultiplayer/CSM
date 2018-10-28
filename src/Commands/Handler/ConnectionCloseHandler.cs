
using CSM.Networking;

namespace CSM.Commands.Handler
{
    class ConnectionCloseHandler : CommandHandler<ConnectionCloseCommand>
    {
        public override byte ID => 2;

        public override void HandleOnClient(ConnectionCloseCommand command)
        {
            MultiplayerManager.Instance.CurrentClient.Disconnect();
        }

        public override void HandleOnServer(ConnectionCloseCommand command, Player player)
        {
            MultiplayerManager.Instance.CurrentServer.HandlePlayerDisconnect(player);
        }
    }
}
