using ColossalFramework;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class DemAndDisplayedHandler : CommandHandler<DemandDisplayedCommand>
    {
        public override byte ID => 106;

        public override void HandleOnClient(DemandDisplayedCommand command)
        {
            Singleton<ZoneManager>.instance.m_residentialDemand = command.ResidentialDemand;
            Singleton<ZoneManager>.instance.m_commercialDemand = command.CommercialDemand;
            Singleton<ZoneManager>.instance.m_workplaceDemand = command.WorkplaceDemand;
        }

        public override void HandleOnServer(DemandDisplayedCommand command, Player player)
        {
        }
    }
}