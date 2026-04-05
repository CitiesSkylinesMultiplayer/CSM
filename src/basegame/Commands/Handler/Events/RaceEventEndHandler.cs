using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Events;
using ColossalFramework;

namespace CSM.BaseGame.Commands.Handler.Events
{
    public class RaceEventEndHandler : CommandHandler<RaceEventEndCommand>
    {
        protected override void Handle(RaceEventEndCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            ref EventData eventData = ref Singleton<EventManager>.instance.m_events.m_buffer[command.Event];
            RaceEventAI raceAI = eventData.Info.m_eventAI as RaceEventAI;
            if (raceAI != null)
            {
                Log.Info($"[CSM Race] Ending race event {command.Event}");
                ReflectionHelper.Call(raceAI, "EndRace", command.Event);
            }
            else
            {
                Log.Warn($"[CSM Race] Received RaceEventEndCommand for non-race event {command.Event}");
            }

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
