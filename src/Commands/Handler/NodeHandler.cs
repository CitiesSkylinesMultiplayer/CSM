using ColossalFramework;
using CSM.Networking;
using System.Collections.Generic;
using CSM.Helpers;

namespace CSM.Commands.Handler
{
    public class NodeHandler : CommandHandler<NodeCommand>
    {
        public override byte ID => 109;

        public override void HandleOnClient(NodeCommand command)
        {
            Extensions.NodeAndSegmentExtension._NetSegmentLocked = true;
            lock (Extensions.NodeAndSegmentExtension._netSegment)
            {
                NetInfo netinfo = PrefabCollection<NetInfo>.GetPrefab(command.InfoIndex);
                lock (Extensions.NodeAndSegmentExtension.NodeIDDictionary)
                {
                    lock (Extensions.NodeAndSegmentExtension.VectorDictionary)
                    {
                        Singleton<NetManager>.instance.CreateNode(out ushort node, ref Singleton<SimulationManager>.instance.m_randomizer, netinfo, command.Position, Singleton<SimulationManager>.instance.m_currentBuildIndex);
                        Extensions.NodeAndSegmentExtension.NodeIDDictionary.Add((ushort)command.NodeID, node); //Adds the NodeID recived and the NodeID generated on NodeCreation.
                        Extensions.NodeAndSegmentExtension.VectorDictionary.Add(command.Position, node);
                    }
                }
            }
            Extensions.NodeAndSegmentExtension._NetSegmentLocked = false;
        }

        public override void HandleOnServer(NodeCommand command, Player player)
        {
            Extensions.NodeAndSegmentExtension._NetSegmentLocked = true;
            lock (Extensions.NodeAndSegmentExtension._netSegment)
            {
                NetInfo netinfo = PrefabCollection<NetInfo>.GetPrefab(command.InfoIndex);
                lock (Extensions.NodeAndSegmentExtension.NodeIDDictionary)
                {
                    lock (Extensions.NodeAndSegmentExtension.VectorDictionary)
                    {
                        Singleton<NetManager>.instance.CreateNode(out ushort node, ref Singleton<SimulationManager>.instance.m_randomizer, netinfo, command.Position, Singleton<SimulationManager>.instance.m_currentBuildIndex);
                        Extensions.NodeAndSegmentExtension.NodeIDDictionary.Add((ushort)command.NodeID, node);
                        Extensions.NodeAndSegmentExtension.VectorDictionary.Add(command.Position, node);
                    }
                }
            }
            Extensions.NodeAndSegmentExtension._NetSegmentLocked = false;
        }
    }
}