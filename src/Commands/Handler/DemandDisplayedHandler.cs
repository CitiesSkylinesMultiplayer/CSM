using ColossalFramework;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class DemandDisplayedHandler : CommandHandler<DemandDisplayedCommand>
    {
        public override void Handle(DemandDisplayedCommand command)
        {
            // Don't handle on server
            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
                return;

            Singleton<ZoneManager>.instance.m_residentialDemand = command.ResidentialDemand;
            Singleton<ZoneManager>.instance.m_commercialDemand = command.CommercialDemand;
            Singleton<ZoneManager>.instance.m_workplaceDemand = command.WorkplaceDemand;
        }
    }
}
