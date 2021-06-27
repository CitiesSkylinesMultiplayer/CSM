using ColossalFramework.UI;
using CSM.API.Commands;
using CSM.Commands.Data.Events;
using CSM.Helpers;

namespace CSM.Commands.Handler.Events
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
                UISlider slider = ReflectionHelper.GetAttr<UISlider>(panel, "m_securityBudgetSlider");
                SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                {
                    slider.value = command.Budget;
                });
            }

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
