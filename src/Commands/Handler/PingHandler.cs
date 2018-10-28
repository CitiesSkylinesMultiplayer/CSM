
using CSM.Networking;
using System;

namespace CSM.Commands.Handler
{
    class PingHandler : CommandHandler<PingCommand>
    {
        public override byte ID => 3;

        public override void HandleOnClient(PingCommand command)
        {
            // Update the last server ping
            MultiplayerManager.Instance.CurrentClient.UpdatePing();
            // Send back a ping event
            Command.SendToServer(new PingCommand());
        }

        public override void HandleOnServer(PingCommand command, Player player)
        {
            player.LastPing = DateTime.UtcNow;
        }
    }
}
