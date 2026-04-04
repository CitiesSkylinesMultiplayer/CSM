using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Disasters;
using ColossalFramework;
using UnityEngine;

namespace CSM.BaseGame.Commands.Handler.Disasters
{
    public class DisasterHandler : CommandHandler<DisasterCommand>
    {
        protected override void Handle(DisasterCommand command)
        {
            DisasterInfo info = PrefabCollection<DisasterInfo>.GetPrefab(command.InfoIndex);

            if (info == null)
            {
                Log.Warn($"[CSM] Received DisasterCommand with invalid InfoIndex: {command.InfoIndex}");
                return;
            }

            Log.Info($"[CSM] Triggering disaster {info.name} at {command.Position}");

            IgnoreHelper.Instance.StartIgnore();

            DisasterManager instance = Singleton<DisasterManager>.instance;
            if (instance.CreateDisaster(out ushort disasterID, info))
            {
                instance.m_disasters.m_buffer[disasterID].m_targetPosition = command.Position;
                instance.m_disasters.m_buffer[disasterID].m_angle = command.Angle;
                instance.m_disasters.m_buffer[disasterID].m_intensity = (byte)command.Intensity;
                instance.m_disasters.m_buffer[disasterID].m_randomSeed = command.RandomSeed;
                instance.m_disasters.m_buffer[disasterID].m_flags |= DisasterData.Flags.SelfTrigger;

                info.m_disasterAI.StartNow(disasterID, ref instance.m_disasters.m_buffer[disasterID]);
            }

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
