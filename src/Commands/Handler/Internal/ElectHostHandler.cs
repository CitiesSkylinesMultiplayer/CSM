using CSM.Commands.Data.Internal;
using CSM.Networking;
using CSM.Util;

namespace CSM.Commands.Handler.Internal
{
    public class ElectHostHandler : CommandHandler<ElectHostCommand>
    {
        protected override void Handle(ElectHostCommand command)
        {
            if (!MultiplayerManager.Instance.IsClient())
            {
                Log.Debug("ElectHostCommand received but this is not a client...");
                return;
            }

            Log.Debug("I will be the host...");
            MultiplayerManager.Instance.ElectAsHost();
        }
    }
}
