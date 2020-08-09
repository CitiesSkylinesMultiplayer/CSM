using ColossalFramework.Threading;
using CSM.Commands.Data.Internal;
using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Status;
using CSM.Panels;

namespace CSM.Commands.Handler.Internal
{
    public class ConnectionResultHandler : CommandHandler<ConnectionResultCommand>
    {
        // Class logger
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public ConnectionResultHandler()
        {
            TransactionCmd = false;
        }

        protected override void Handle(ConnectionResultCommand command)
        {
            // We only want this message while connecting
            if (MultiplayerManager.Instance.CurrentClient.Status != ClientStatus.Connecting)
                return;

            // If we are allowed to connect
            if (command.Success)
            {
                // Log and set that we are connected.
                _logger.Info("Successfully connected to server. Downloading world...");
                MultiplayerManager.Instance.CurrentClient.ClientPlayer = new Player();
                MultiplayerManager.Instance.CurrentClient.Status = ClientStatus.Downloading;
                MultiplayerManager.Instance.CurrentClient.ClientId = command.ClientId;
            }
            else
            {
                _logger.Info($"Could not connect: {command.Reason}");
                MultiplayerManager.Instance.CurrentClient.ConnectionMessage = command.Reason;
                MultiplayerManager.Instance.CurrentClient.Disconnect();
                if (command.Reason.Contains("DLC")) // No other way to detect if we should display the box
                {
                    DLCHelper.DLCComparison compare = DLCHelper.Compare(command.DLCBitMask, DLCHelper.GetOwnedDLCs());

                    ThreadHelper.dispatcher.Dispatch(() =>
                    {
                        MessagePanel panel = PanelManager.ShowPanel<MessagePanel>();
                        panel.DisplayDlcMessage(compare);
                    });
                }
            }
        }
    }
}
