using ColossalFramework;
using CSM.Helpers;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class NodeSegmentHandler : CommandHandler<NodeSegmentCommand>
    {
        public override byte ID => 110;

        private ushort StartNode;
        private ushort EndNode;

        public override void HandleOnClient(NodeSegmentCommand command)
        {
            Extensions.NodeAndSegmentExtension._NetSegmentLocked = true;
            lock (Extensions.NodeAndSegmentExtension._netSegment)
            {
                NetInfo netinfo = PrefabCollection<NetInfo>.GetPrefab(command.InfoIndex);
                StartNode = Extensions.NodeAndSegmentExtension.NodeIDDictionary[command.StartNode]; //uses the Dictionary of the Client and Servers different NodeID to convert the recived NodeID the the NodeID of the reciever
                EndNode = Extensions.NodeAndSegmentExtension.NodeIDDictionary[command.EndNode];
                lock (Extensions.NodeAndSegmentExtension.StartEndNodeDictionary)
                {
                    Singleton<NetManager>.instance.CreateSegment(out ushort id, ref Singleton<SimulationManager>.instance.m_randomizer, netinfo, StartNode, EndNode, command.StartDirection, command.EndDirection, Singleton<SimulationManager>.instance.m_currentBuildIndex, command.ModifiedIndex, false);
                    StartEndNode startEndNode = new StartEndNode(Singleton<NetManager>.instance.m_nodes.m_buffer[StartNode].m_position, Singleton<NetManager>.instance.m_nodes.m_buffer[EndNode].m_position);
                    Extensions.NodeAndSegmentExtension.StartEndNodeDictionary.Add(startEndNode, id);
                }
            }
            Extensions.NodeAndSegmentExtension._NetSegmentLocked = false;
        }

        public override void HandleOnServer(NodeSegmentCommand command, Player player)
        {
            Extensions.NodeAndSegmentExtension._NetSegmentLocked = true;
            lock (Extensions.NodeAndSegmentExtension._netSegment)
            {
                NetInfo netinfo = PrefabCollection<NetInfo>.GetPrefab(command.InfoIndex);
                var StartNode = Extensions.NodeAndSegmentExtension.NodeIDDictionary[command.StartNode]; //uses the Dictionary of the Client and Servers different NodeID to convert the recived NodeID the the NodeID of the reciever
                var EndNode = Extensions.NodeAndSegmentExtension.NodeIDDictionary[command.EndNode];
                lock (Extensions.NodeAndSegmentExtension.StartEndNodeDictionary)
                {
                    Singleton<NetManager>.instance.CreateSegment(out ushort id, ref Singleton<SimulationManager>.instance.m_randomizer, netinfo, StartNode, EndNode, command.StartDirection, command.EndDirection, Singleton<SimulationManager>.instance.m_currentBuildIndex, command.ModifiedIndex, false);
                    StartEndNode startEndNode = new StartEndNode(Singleton<NetManager>.instance.m_nodes.m_buffer[StartNode].m_position, Singleton<NetManager>.instance.m_nodes.m_buffer[EndNode].m_position);
                    Extensions.NodeAndSegmentExtension.StartEndNodeDictionary.Add(startEndNode, id);
                }
            }
            Extensions.NodeAndSegmentExtension._NetSegmentLocked = false;
        }
    }
}