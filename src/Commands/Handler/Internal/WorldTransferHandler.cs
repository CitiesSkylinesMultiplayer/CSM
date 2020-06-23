using CSM.Commands.Data.Internal;
using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Status;

namespace CSM.Commands.Handler.Internal
{
    public class WorldTransferHandler : CommandHandler<WorldTransferCommand>
    {
        // Class logger
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public WorldTransferHandler()
        {
            TransactionCmd = false;
        }

        protected override void Handle(WorldTransferCommand command)
        {
            if (MultiplayerManager.Instance.CurrentClient.Status == ClientStatus.Downloading)
            {
                _logger.Info("World has been received, preparing to load world.");

                MultiplayerManager.Instance.CurrentClient.Status = ClientStatus.Loading;

                MultiplayerManager.Instance.CurrentClient.StopMainMenuEventProcessor();

                SaveHelpers.LoadLevel(command.World);

                MultiplayerManager.Instance.UnblockGame(true);

                // See LoadingExtension for events after level loaded
            }
        }
    }
}
