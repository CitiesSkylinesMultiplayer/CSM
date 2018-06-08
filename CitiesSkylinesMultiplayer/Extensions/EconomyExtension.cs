using CitiesSkylinesMultiplayer.Networking;
using ICities;

namespace CitiesSkylinesMultiplayer.Extensions
{
    /// <summary>
    /// Handles game economy. If client, this class will get updated 
    /// info from server, if server, this class will send info to client.
    /// </summary>
    public class EconomyExtension : EconomyExtensionBase
    {
        public override long OnUpdateMoneyAmount(long internalMoneyAmount)
        {
            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
            {

            }

            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
            {
                // Todo: Transmit this info to the server via client
                return internalMoneyAmount;
            }

            return internalMoneyAmount;
        }
    }
}
