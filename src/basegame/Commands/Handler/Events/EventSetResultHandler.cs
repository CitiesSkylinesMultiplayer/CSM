using CSM.API.Commands;
using CSM.BaseGame.Commands.Data.Events;

namespace CSM.BaseGame.Commands.Handler.Events
{
    public class EventSetResultHandler : CommandHandler<EventSetResultCommand>
    {
        protected override void Handle(EventSetResultCommand command)
        {
            ref EventData data = ref EventManager.instance.m_events.m_buffer[command.Event];
            if (command.Result)
            {
                data.m_flags |= EventData.Flags.Success;
                data.m_flags &= ~EventData.Flags.Failure;
            }
            else
            {
                data.m_flags |= EventData.Flags.Failure;
                data.m_flags &= ~EventData.Flags.Success;
            }
        }
    }
}
