using CSM.Commands.Data.Internal;
using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Status;
using System.Threading;

namespace CSM.Commands.Handler.Internal
{
    class WorldTransferHandler : CommandHandler<WorldTransferCommand>
    {
        // Class logger
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public WorldTransferHandler() 
        {
            TransactionCmd = false;
        }

        protected override void Handle(WorldTransferCommand command)
        {
            new Thread(() =>
            {
                if (MultiplayerManager.Instance.CurrentClient.Status == ClientStatus.Downloading)
                {
                    _logger.Info("World has been received, preparing to load world.");

                    MultiplayerManager.Instance.CurrentClient.Status = ClientStatus.Loading;

                    MultiplayerManager.Instance.CurrentClient.StopMainMenuEventProcessor();

                    SaveHelpers.LoadLevel(command.World);

                    // See LoadingExtension for events after level loaded
                }
            }).Start();
        }
    }
}
