using CSM.Commands.Data.Internal;
using CSM.Server.Util;

namespace CSM.Commands.Handler.Internal
{
    public class WorldTransferHandler : CommandHandler<WorldTransferCommand>
    {
        public WorldTransferHandler()
        {
            TransactionCmd = false;
        }

        protected override void Handle(WorldTransferCommand command)
        {
            Log.Info("World has been received, saving world.");
            SaveHelpers.SaveWorkFile(command.World);
            Log.Info("Done saving world.");
        }
    }
}
