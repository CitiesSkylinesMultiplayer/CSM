using CSM.Commands.Data.Internal;
using CSM.Helpers;
using CSM.Networking;
using CSM.Networking.Status;
using System.Threading;

namespace CSM.Commands.Handler.Internal
{
    class WorldTransferHandler : CommandHandler<WorldTransferCommand>
    {
        public WorldTransferHandler() 
        {
            TransactionCmd = false;
        }

        protected override void Handle(WorldTransferCommand command)
        {
            new Thread(() =>
            {
                SaveHelpers.SaveWorldFile(command.World);
                MultiplayerManager.Instance.CurrentClient.Status = ClientStatus.Loading;
            }).Start();
        }
    }
}
