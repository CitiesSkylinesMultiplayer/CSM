using ColossalFramework;
using CSM.Helpers;
using CSM.Networking;
using Harmony;
using System.Reflection;

namespace CSM.Commands.Handler
{
    public class NodeCreateHandler : CommandHandler<NodeCreateCommand>
    {
        private MethodInfo _initializeNode;

        public NodeCreateHandler()
        {
            _initializeNode = typeof(NetManager).GetMethod("InitializeNode", AccessTools.all);
        }

        public override byte ID => CommandIds.NodeCreateCommand;

        public override void HandleOnServer(NodeCreateCommand command, Player player) => HandleCreateNode(command);

        public override void HandleOnClient(NodeCreateCommand command) => HandleCreateNode(command);

        private void HandleCreateNode(NodeCreateCommand command)
        {
            NetInfo info = PrefabCollection<NetInfo>.GetPrefab(command.InfoIndex);
            ushort node = command.NodeId;
            // Don't allow this node id to be taken (=CreateItem at the creator)
            Singleton<NetManager>.instance.m_nodes.RemoveUnused(node);

            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_position = command.Position;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_flags = NetNode.Flags.Created;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].Info = info;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_buildIndex = Singleton<SimulationManager>.instance.m_currentBuildIndex;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_building = 0;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_lane = 0u;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_nextLaneNode = 0;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_nextBuildingNode = 0;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_nextGridNode = 0;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_laneOffset = 0;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_elevation = 0;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_heightOffset = 0;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_problems = Notification.Problem.None;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_transportLine = 0;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_connectCount = 0;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_maxWaitTime = 0;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_coverage = 0;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_tempCounter = 0;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_finalCounter = 0;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_targetCitizens = 0;

            info.m_netAI.CreateNode(node, ref Singleton<NetManager>.instance.m_nodes.m_buffer[node]);
            _initializeNode.Invoke(Singleton<NetManager>.instance, new object[] { node, Singleton<NetManager>.instance.m_nodes.m_buffer[node] });

            Singleton<NetManager>.instance.UpdateNode(node);
            Singleton<NetManager>.instance.UpdateNodeColors(node);
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].UpdateBounds(node);
            Singleton<NetManager>.instance.m_nodeCount = ((int)Singleton<NetManager>.instance.m_nodes.ItemCount()) - 1;

            Singleton<SimulationManager>.instance.m_currentBuildIndex++;
        }
    }
}