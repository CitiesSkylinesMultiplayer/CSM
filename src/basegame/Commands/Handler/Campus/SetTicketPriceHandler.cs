using ColossalFramework;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Campus;
using CSM.BaseGame.Helpers;

namespace CSM.BaseGame.Commands.Handler.Campus
{
    public class SetTicketPriceHandler : CommandHandler<SetTicketPriceCommand>
    {
        protected override void Handle(SetTicketPriceCommand command)
        {
            Singleton<DistrictManager>.instance.m_parks.m_buffer[command.Campus].m_ticketPrice = command.Price;

            // Refresh panel
            if (InfoPanelHelper.IsPark(typeof(CampusWorldInfoPanel), command.Campus, out WorldInfoPanel panel))
            {
                SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                {
                    ReflectionHelper.Call((CampusWorldInfoPanel)panel, "OnSetTarget");
                });
            }
        }
    }
}
