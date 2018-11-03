
using ColossalFramework;
using CSM.Networking;
using CSM.Helpers;
using System.Threading;

namespace CSM.Commands.Handler
{
	class NodeSegmentHandler : CommandHandler<NodeSegmentCommand>
	{
		public override byte ID => 110;

		ushort StartNode;
		ushort EndNode;

		public override void HandleOnClient(NodeSegmentCommand command)
		{
			Extensions.NodeandSegmentExtension._NetSegmentLocked = true;
			lock (Extensions.NodeandSegmentExtension._netSegment)
			{
				
				NetInfo netinfo = PrefabCollection<NetInfo>.GetPrefab(command.InfoIndex);
				StartNode = Extensions.NodeandSegmentExtension.NodeIDDictionary[command.StartNode]; //uses the Dictionary of the Client and Servers different NodeID to convert the recived NodeID the the NodeID of the reciever
				EndNode = Extensions.NodeandSegmentExtension.NodeIDDictionary[command.EndNode];
				lock (Extensions.NodeandSegmentExtension.StartEndNodeDictionary)
				{
					Singleton<NetManager>.instance.CreateSegment(out ushort id, ref Singleton<SimulationManager>.instance.m_randomizer, netinfo, StartNode, EndNode, command.StartDirection, command.EndDirection, Singleton<SimulationManager>.instance.m_currentBuildIndex, command.ModifiedIndex, false);
					StartEndNode startEndNode = new StartEndNode(Singleton<NetManager>.instance.m_nodes.m_buffer[StartNode].m_position, Singleton<NetManager>.instance.m_nodes.m_buffer[EndNode].m_position);
					Extensions.NodeandSegmentExtension.StartEndNodeDictionary.Add(startEndNode, id);
				}
				
			}
			Extensions.NodeandSegmentExtension._NetSegmentLocked = false;

		}

		public override void HandleOnServer(NodeSegmentCommand command, Player player)
		{
			Extensions.NodeandSegmentExtension._NetSegmentLocked = true;
			lock (Extensions.NodeandSegmentExtension._netSegment)
			{
				NetInfo netinfo = PrefabCollection<NetInfo>.GetPrefab(command.InfoIndex);
				var StartNode = Extensions.NodeandSegmentExtension.NodeIDDictionary[command.StartNode]; //uses the Dictionary of the Client and Servers different NodeID to convert the recived NodeID the the NodeID of the reciever
				var EndNode = Extensions.NodeandSegmentExtension.NodeIDDictionary[command.EndNode];
				lock (Extensions.NodeandSegmentExtension.StartEndNodeDictionary)
				{
					Singleton<NetManager>.instance.CreateSegment(out ushort id, ref Singleton<SimulationManager>.instance.m_randomizer, netinfo, StartNode, EndNode, command.StartDirection, command.EndDirection, Singleton<SimulationManager>.instance.m_currentBuildIndex, command.ModifiedIndex, false);
					StartEndNode startEndNode = new StartEndNode(Singleton<NetManager>.instance.m_nodes.m_buffer[StartNode].m_position, Singleton<NetManager>.instance.m_nodes.m_buffer[EndNode].m_position);
					Extensions.NodeandSegmentExtension.StartEndNodeDictionary.Add(startEndNode, id);

				}
				
			}
			Extensions.NodeandSegmentExtension._NetSegmentLocked = false;
		}
	}
}
