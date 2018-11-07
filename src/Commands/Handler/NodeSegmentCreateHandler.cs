using ColossalFramework;
using CSM.Helpers;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class NodeSegmentCreateHandler : CommandHandler<NodeSegmentCreateCommand>
    {
        public override byte ID => 110;

        private ushort _startNode;
        private ushort _endNode;

        public override void HandleOnClient(NodeSegmentCreateCommand command)
        {
            //Extensions.NodeAndSegmentExtension.NetSegmentLocked = true;

            NetInfo netinfo = PrefabCollection<NetInfo>.GetPrefab(command.InfoIndex);
            _startNode = Extensions.NodeAndSegmentExtension.NodeIDDictionary[command.StartNode]; //uses the Dictionary of the Client and Servers different NodeID to convert the recived NodeID the the NodeID of the reciever
            _endNode = Extensions.NodeAndSegmentExtension.NodeIDDictionary[command.EndNode];
            StartEndNode startEndNode = new StartEndNode(Singleton<NetManager>.instance.m_nodes.m_buffer[_startNode].m_position, Singleton<NetManager>.instance.m_nodes.m_buffer[_endNode].m_position);
            Extensions.NodeAndSegmentExtension.StartEndNodeDictionary.Add(startEndNode, 100); // add dummy segment to hinder occilation
            Singleton<NetManager>.instance.CreateSegment(out ushort id, ref Singleton<SimulationManager>.instance.m_randomizer, netinfo, _startNode, _endNode, command.StartDirection, command.EndDirection, Singleton<SimulationManager>.instance.m_currentBuildIndex, command.ModifiedIndex, false);
            Extensions.NodeAndSegmentExtension.StartEndNodeDictionary[startEndNode] = id;
        }

        public override void HandleOnServer(NodeSegmentCreateCommand command, Player player)
        {
            //Extensions.NodeAndSegmentExtension.NetSegmentLocked = true;
            NetInfo netinfo = PrefabCollection<NetInfo>.GetPrefab(command.InfoIndex);
            var startNode = Extensions.NodeAndSegmentExtension.NodeIDDictionary[command.StartNode]; //uses the Dictionary of the Client and Servers different NodeID to convert the recived NodeID the the NodeID of the reciever
            var endNode = Extensions.NodeAndSegmentExtension.NodeIDDictionary[command.EndNode];
            StartEndNode startEndNode = new StartEndNode(Singleton<NetManager>.instance.m_nodes.m_buffer[startNode].m_position, Singleton<NetManager>.instance.m_nodes.m_buffer[endNode].m_position);
            Extensions.NodeAndSegmentExtension.StartEndNodeDictionary.Add(startEndNode, 100); // add dummy segment to hinder occilation
            Singleton<NetManager>.instance.CreateSegment(out ushort id, ref Singleton<SimulationManager>.instance.m_randomizer, netinfo, startNode, endNode, command.StartDirection, command.EndDirection, Singleton<SimulationManager>.instance.m_currentBuildIndex, command.ModifiedIndex, false);
            Extensions.NodeAndSegmentExtension.StartEndNodeDictionary[startEndNode] = id;
        }
    }
}