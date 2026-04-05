using System;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Events;
using ColossalFramework;

namespace CSM.BaseGame.Commands.Handler.Events
{
    public class RaceEventCancelHandler : CommandHandler<RaceEventCancelCommand>
    {
        protected override void Handle(RaceEventCancelCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            ref EventData eventData = ref Singleton<EventManager>.instance.m_events.m_buffer[command.Event];
            RaceEventAI raceAI = eventData.Info.m_eventAI as RaceEventAI;
            if (raceAI != null)
            {
                Log.Info($"[CSM Race] Cancelling race event {command.Event}");
                Type[] types = new Type[] { typeof(ushort), typeof(EventData).MakeByRefType() };
                object[] param = new object[] { command.Event, eventData };
                ReflectionHelper.Call(raceAI, "Cancel", types, param);
                eventData = (EventData)param[1];
            }
            else
            {
                Log.Warn($"[CSM Race] Received RaceEventCancelCommand for non-race event {command.Event}");
            }

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
