using ColossalFramework;
using CSM.Helpers;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class NodeSegmentCreateHandler : CommandHandler<NodeSegmentCreateCommand>
    {
        public override byte ID => 110;

        private ushort StartNode;
        private ushort EndNode;

        public override void HandleOnClient(NodeSegmentCreateCommand command)
        {
            Extensions.NodeAndSegmentExtension._NetSegmentLocked = true;

            NetInfo netinfo = PrefabCollection<NetInfo>.GetPrefab(command.InfoIndex);
            StartNode = Extensions.NodeAndSegmentExtension.NodeIDDictionary[command.StartNode]; //uses the Dictionary of the Client and Servers different NodeID to convert the recived NodeID the the NodeID of the reciever
            EndNode = Extensions.NodeAndSegmentExtension.NodeIDDictionary[command.EndNode];
            StartEndNode startEndNode = new StartEndNode(Singleton<NetManager>.instance.m_nodes.m_buffer[StartNode].m_position, Singleton<NetManager>.instance.m_nodes.m_buffer[EndNode].m_position);
            Extensions.NodeAndSegmentExtension.StartEndNodeDictionary.Add(startEndNode, 100); // add dummy segment to hinder occilation
            Singleton<NetManager>.instance.CreateSegment(out ushort id, ref Singleton<SimulationManager>.instance.m_randomizer, netinfo, StartNode, EndNode, command.StartDirection, command.EndDirection, Singleton<SimulationManager>.instance.m_currentBuildIndex, command.ModifiedIndex, false);
            Extensions.NodeAndSegmentExtension.StartEndNodeDictionary[startEndNode] = id;
        }

        public override void HandleOnServer(NodeSegmentCreateCommand command, Player player)
        {
            Extensions.NodeAndSegmentExtension._NetSegmentLocked = true;
            NetInfo netinfo = PrefabCollection<NetInfo>.GetPrefab(command.InfoIndex);
            var StartNode = Extensions.NodeAndSegmentExtension.NodeIDDictionary[command.StartNode]; //uses the Dictionary of the Client and Servers different NodeID to convert the recived NodeID the the NodeID of the reciever
            var EndNode = Extensions.NodeAndSegmentExtension.NodeIDDictionary[command.EndNode];
            StartEndNode startEndNode = new StartEndNode(Singleton<NetManager>.instance.m_nodes.m_buffer[StartNode].m_position, Singleton<NetManager>.instance.m_nodes.m_buffer[EndNode].m_position);
            Extensions.NodeAndSegmentExtension.StartEndNodeDictionary.Add(startEndNode, 100); // add dummy segment to hinder occilation
            Singleton<NetManager>.instance.CreateSegment(out ushort id, ref Singleton<SimulationManager>.instance.m_randomizer, netinfo, StartNode, EndNode, command.StartDirection, command.EndDirection, Singleton<SimulationManager>.instance.m_currentBuildIndex, command.ModifiedIndex, false);
            Extensions.NodeAndSegmentExtension.StartEndNodeDictionary[startEndNode] = id;
        }
    }
}