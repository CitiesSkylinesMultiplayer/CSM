
using CSM.Networking;
using CSM.Networking.Status;

namespace CSM.Commands.Handler
{
    class ConnectionResultHandler : CommandHandler<ConnectionResultCommand>
    {
        public override byte ID => 1;

        public override void HandleOnClient(ConnectionResultCommand command)
        {
            // We only want this message while connecting
            if (MultiplayerManager.Instance.CurrentClient.Status != ClientStatus.Connecting)
                return;

            // If we are allowed to connect
            if (command.Success)
            {
                // Log and set that we are connected.
                CSM.Log($"Successfully connected to server.");
                MultiplayerManager.Instance.CurrentClient.UpdatePing();
                MultiplayerManager.Instance.CurrentClient.Status = ClientStatus.Connected;
            }
            else
            {
                CSM.Log($"Could not connect: {command.Reason}");
                MultiplayerManager.Instance.CurrentClient.ConnectionMessage = $"Could not connect: {command.Reason}";
                MultiplayerManager.Instance.CurrentClient.Disconnect();
            }
        }

        public override void HandleOnServer(ConnectionResultCommand command, Player player) { }
    }
}
