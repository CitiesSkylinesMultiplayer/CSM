using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Events;
using CSM.BaseGame.Helpers;

namespace CSM.BaseGame.Commands.Handler.Events
{
    public class EventSetConcertTicketPriceHandler : CommandHandler<EventSetConcertTicketPriceCommand>
    {
        protected override void Handle(EventSetConcertTicketPriceCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            ConcertAI concertAI = PrefabCollection<EventInfo>.GetLoaded(command.Event).GetAI() as ConcertAI;
            EventData data = new EventData();
            if (concertAI != null) concertAI.SetTicketPrice(0, ref data, command.Price);

            IgnoreHelper.Instance.EndIgnore();

            if (InfoPanelHelper.IsBuilding(typeof(FestivalPanel), command.Building, out WorldInfoPanel panel))
            {
                SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                {
                    ReflectionHelper.Call((FestivalPanel)panel, "RefreshTicketPrices");
                });
            }
        }
    }
}
