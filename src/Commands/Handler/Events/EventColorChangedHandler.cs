using ColossalFramework.UI;
using CSM.Commands.Data.Events;
using CSM.Helpers;
using System;

namespace CSM.Commands.Handler.Events
{
    public class EventColorChangedHandler : CommandHandler<EventColorChangedCommand>
    {
        protected override void Handle(EventColorChangedCommand command)
        {
            IgnoreHelper.StartIgnore();

            ref EventData eventData = ref EventManager.instance.m_events.m_buffer[command.Event];
            eventData.Info.m_eventAI.SetColor(command.Event, ref eventData, command.Color);

            Type aiType = eventData.Info.m_eventAI.GetType();
            if (aiType == typeof(RocketLaunchAI))
            {
                ChirpXPanel panel = UIView.library.Get<ChirpXPanel>(typeof(ChirpXPanel).Name);

                if (ReflectionHelper.GetAttr<ushort>(panel, "m_currentEventID") == command.Event)
                {
                    SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                    {
                        panel.rocketColor = command.Color;
                    });
                    return;
                }
            }
            else if (aiType == typeof(ConcertAI))
            {
                if (InfoPanelHelper.IsEventBuilding(typeof(FestivalPanel), command.Event, out WorldInfoPanel panel))
                {
                    SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                    {
                        ReflectionHelper.GetAttr<UIColorField>(panel, "m_colorField").selectedColor = command.Color;
                    });
                }
            }
            else if (aiType == typeof(SportMatchAI))
            {
                if (InfoPanelHelper.IsEventBuilding(typeof(FootballPanel), command.Event, out WorldInfoPanel panel))
                {
                    SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                    {
                        ((FootballPanel)panel).teamColor = command.Color;
                    });
                }
            }

            IgnoreHelper.EndIgnore();
        }
    }
}
