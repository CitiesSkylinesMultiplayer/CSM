using CSM.Commands.Data.Internal;
using CSM.Helpers;
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
            }).Start();
        }
    }
}
