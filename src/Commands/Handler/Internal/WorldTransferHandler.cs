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

                    SaveHelpers.LoadLevel(command.World);

                    MultiplayerManager.Instance.CurrentClient.Status = ClientStatus.Connected;

                    // TODO, send packet that tells the server that we are now loaded, server
                    // should then set the client status to ready, (and maybe send over all packets
                    // that is missed, that we don't have to pause the game for everyone on client
                    // connect).
                }
            }).Start();
        }
    }
}
