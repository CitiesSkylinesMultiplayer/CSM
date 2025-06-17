using ColossalFramework.UI;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Events;
using CSM.BaseGame.Helpers;

namespace CSM.BaseGame.Commands.Handler.Events
{
    public class EventSetSecurityBudgetHandler : CommandHandler<EventSetSecurityBudgetCommand>
    {
        protected override void Handle(EventSetSecurityBudgetCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            ref EventData eventData = ref EventManager.instance.m_events.m_buffer[command.Event];
            eventData.Info.m_eventAI.SetSecurityBudget(command.Event, ref eventData, command.Budget);

            if (InfoPanelHelper.IsEventBuilding(typeof(FestivalPanel), command.Event, out WorldInfoPanel panel))
            {
                SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                {
                    ReflectionHelper.Call(panel, "OnSetTarget");
                });
            }

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
