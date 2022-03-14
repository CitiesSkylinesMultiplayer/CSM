using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Events;

namespace CSM.BaseGame.Commands.Handler.Events
{
    public class EventActivateHandler : CommandHandler<EventActivateCommand>
    {
        protected override void Handle(EventActivateCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            ref EventData eventData = ref EventManager.instance.m_events.m_buffer[command.Event];
            eventData.Info.m_eventAI.Activate(command.Event, ref eventData);

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
