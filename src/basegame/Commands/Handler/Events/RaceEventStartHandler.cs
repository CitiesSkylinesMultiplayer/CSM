using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Events;
using ColossalFramework;

namespace CSM.BaseGame.Commands.Handler.Events
{
    public class RaceEventStartHandler : CommandHandler<RaceEventStartCommand>
    {
        protected override void Handle(RaceEventStartCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            ref EventData eventData = ref Singleton<EventManager>.instance.m_events.m_buffer[command.Event];
            RaceEventAI raceAI = eventData.Info.m_eventAI as RaceEventAI;
            if (raceAI != null)
            {
                Log.Info($"[CSM Race] Starting race event {command.Event}");
                ReflectionHelper.Call(raceAI, "StartRace", command.Event);
            }
            else
            {
                Log.Warn($"[CSM Race] Received RaceEventStartCommand for non-race event {command.Event}");
            }

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
