using CSM.Networking;
using CSM.Networking.Status;
using CSM.Panels;
using CSM.Localisation;

namespace CSM.Commands.Handler
{
    public class ConnectionResultHandler : CommandHandler<ConnectionResultCommand>
    {
        // Class logger
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public ConnectionResultHandler()
        {
            TransactionCmd = false;
        }

        public override void Handle(ConnectionResultCommand command)
        {
            // We only want this message while connecting
            if (MultiplayerManager.Instance.CurrentClient.Status != ClientStatus.Connecting)
                return;

            // If we are allowed to connect
            if (command.Success)
            {
                // Log and set that we are connected.
                _logger.Info("Successfully connected to server.");
                ChatLogPanel.PrintGameMessage($"{Translation.PullTranslation("SuccessfullyConnected")}.");
                MultiplayerManager.Instance.CurrentClient.Status = ClientStatus.Connected;
                MultiplayerManager.Instance.CurrentClient.ClientId = command.ClientId;
            }
            else
            {
                _logger.Info($"Could not connect: {command.Reason}");
                MultiplayerManager.Instance.CurrentClient.ConnectionMessage = command.Reason;
                MultiplayerManager.Instance.CurrentClient.Disconnect();
            }
        }
    }
}
