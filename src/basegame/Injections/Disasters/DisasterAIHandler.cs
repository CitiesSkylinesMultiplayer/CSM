using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Disasters;
using HarmonyLib;

namespace CSM.BaseGame.Injections.Disasters
{
    [HarmonyPatch(typeof(DisasterAI))]
    [HarmonyPatch("StartNow")]
    public class DisasterAIHandler
    {
        public static void Prefix(ushort disasterID, ref DisasterData data, DisasterAI __instance)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            if (Command.CurrentRole == MultiplayerRole.None)
                return;

            // Sync if:
            // 1. It was manually triggered by a player (SelfTrigger flag set).
            // 2. We are the Server (coordinates all disasters, including random ones).
            bool isManual = (data.m_flags & DisasterData.Flags.SelfTrigger) != DisasterData.Flags.None;

            if (isManual || Command.CurrentRole == MultiplayerRole.Server)
            {
                Command.SendToAll(new DisasterCommand
                {
                    InfoIndex = (uint)__instance.m_info.m_prefabDataIndex,
                    Position = data.m_targetPosition,
                    Angle = data.m_angle,
                    Intensity = (int)data.m_intensity,
                    RandomSeed = data.m_randomSeed
                });
            }
        }
    }
}
