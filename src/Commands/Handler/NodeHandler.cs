using ColossalFramework;
using CSM.Networking;
using System.Collections.Generic;
using CSM.Helpers;

namespace CSM.Commands.Handler
{
	class NodeHandler : CommandHandler<NodeCommand>
	{

		public override byte ID => 109;

		public override void HandleOnClient(NodeCommand command)
		{
			Extensions.NodeandSegmentExtension._NetSegmentLocked = true;
			lock (Extensions.NodeandSegmentExtension._netSegment)
			{
				

				NetInfo netinfo = PrefabCollection<NetInfo>.GetPrefab(command.InfoIndex);
				lock (Extensions.NodeandSegmentExtension.NodeIDDictionary)
				{
					lock (Extensions.NodeandSegmentExtension.VectorDictionary)
					{
						Singleton<NetManager>.instance.CreateNode(out ushort node, ref Singleton<SimulationManager>.instance.m_randomizer, netinfo, command.Position, Singleton<SimulationManager>.instance.m_currentBuildIndex);
						Extensions.NodeandSegmentExtension.NodeIDDictionary.Add((ushort)command.NodeID, node); //Adds the NodeID recived and the NodeID generated on NodeCreation.
						Extensions.NodeandSegmentExtension.VectorDictionary.Add(command.Position, node);

					}
				}
				
			}
			Extensions.NodeandSegmentExtension._NetSegmentLocked = false;
		}

		public override void HandleOnServer(NodeCommand command, Player player)
		{
			Extensions.NodeandSegmentExtension._NetSegmentLocked = true;
			lock (Extensions.NodeandSegmentExtension._netSegment)
			{
				

				NetInfo netinfo = PrefabCollection<NetInfo>.GetPrefab(command.InfoIndex);
				lock (Extensions.NodeandSegmentExtension.NodeIDDictionary)
				{
					lock (Extensions.NodeandSegmentExtension.VectorDictionary)
					{
						Singleton<NetManager>.instance.CreateNode(out ushort node, ref Singleton<SimulationManager>.instance.m_randomizer, netinfo, command.Position, Singleton<SimulationManager>.instance.m_currentBuildIndex);
						Extensions.NodeandSegmentExtension.NodeIDDictionary.Add((ushort)command.NodeID, node);
						Extensions.NodeandSegmentExtension.VectorDictionary.Add(command.Position, node);
					}
				}

				
			}
			Extensions.NodeandSegmentExtension._NetSegmentLocked = false;
		}
	}
}
