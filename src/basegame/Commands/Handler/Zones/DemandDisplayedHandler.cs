using CSM.API.Commands;
using CSM.BaseGame.Commands.Data.Zones;

namespace CSM.BaseGame.Commands.Handler.Zones
{
    public class DemandDisplayedHandler : CommandHandler<DemandDisplayedCommand>
    {
        protected override void Handle(DemandDisplayedCommand command)
        {
            // Don't handle on server
            if (Command.CurrentRole == MultiplayerRole.Server)
                return;

            ZoneManager.instance.m_residentialDemand = command.ResidentialDemand;
            ZoneManager.instance.m_commercialDemand = command.CommercialDemand;
            ZoneManager.instance.m_workplaceDemand = command.WorkplaceDemand;
        }
    }
}
