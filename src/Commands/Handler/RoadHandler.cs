
using ColossalFramework;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    class RoadHandler : CommandHandler<RoadCommand>
    {
        public override byte ID => 110;

        public override void HandleOnClient(RoadCommand command)
        {
            NetInfo netinfo = PrefabCollection<NetInfo>.GetPrefab(command.InfoIndex);
            Singleton<NetManager>.instance.CreateSegment(out ushort id, ref Singleton<SimulationManager>.instance.m_randomizer, netinfo, command.StartNode, command.EndNode, command.StartDirection, command.EndDirection, Singleton<SimulationManager>.instance.m_currentBuildIndex, command.ModifiedIndex, false);
        }

        public override void HandleOnServer(RoadCommand command, Player player)
        {
            
        }
    }
}
