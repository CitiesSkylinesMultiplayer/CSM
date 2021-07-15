using CSM.Commands.Data.Zones;
using CSM.Networking;

namespace CSM.Commands.Handler.Zones
{
    public class DemandDisplayedHandler : CommandHandler<DemandDisplayedCommand>
    {
        protected override void Handle(DemandDisplayedCommand command)
        {
            // Don't handle on server
            if (MultiplayerManager.Instance.IsServerOrHost())
                return;

            ZoneManager.instance.m_residentialDemand = command.ResidentialDemand;
            ZoneManager.instance.m_commercialDemand = command.CommercialDemand;
            ZoneManager.instance.m_workplaceDemand = command.WorkplaceDemand;
        }
    }
}
