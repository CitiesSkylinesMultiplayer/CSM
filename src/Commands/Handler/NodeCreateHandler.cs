using ColossalFramework;
using CSM.Networking;
using UnityEngine;

namespace CSM.Commands.Handler
{
    public class NodeCreateHandler : CommandHandler<NodeCreateCommand>
    {
        public override byte ID => 109;

        public override void HandleOnServer(NodeCreateCommand command, Player player) => HandleCreateNode(command);

        public override void HandleOnClient(NodeCreateCommand command) => HandleCreateNode(command);

        private void HandleCreateNode(NodeCreateCommand command)
        {
            lock (Extensions.NodeAndSegmentExtension.NodesCreated) {
            //Extensions.NodeAndSegmentExtension.NetSegmentLocked = true;
            NetInfo info = PrefabCollection<NetInfo>.GetPrefab(command.InfoIndex);
            Extensions.NodeAndSegmentExtension.NodesCreated.Add((ushort)command.NodeId);
            uint node = command.NodeId;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_position = command.Position;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_flags = NetNode.Flags.Created;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].Info = info;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_buildIndex = Singleton<SimulationManager>.instance.m_currentBuildIndex;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_building = 0;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_lane = 0;
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
            info.m_netAI.CreateNode((ushort)node, ref Singleton<NetManager>.instance.m_nodes.m_buffer[node]);
            //Singleton<NetManager>.instance.InitializeNode(node, ref Singleton<NetManager>.instance.m_nodes.m_buffer[node]);
            int num = Mathf.Clamp((int)((Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_position.x / 64f) + 135f), 0, 0x10d);
            int index = (Mathf.Clamp((int)((Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_position.z / 64f) + 135f), 0, 0x10d) * 270) + num;
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_nextGridNode = Singleton<NetManager>.instance.m_nodeGrid[index];
            Singleton<NetManager>.instance.m_nodeGrid[index] = (ushort)node;
            Singleton<NetManager>.instance.UpdateNode((ushort)node);
            Singleton<NetManager>.instance.UpdateNodeColors((ushort)node);
            Singleton<NetManager>.instance.m_nodes.m_buffer[node].UpdateBounds((ushort)node);
            Singleton<NetManager>.instance.m_nodeCount = ((int)Singleton<NetManager>.instance.m_nodes.ItemCount()) - 1;
            }



            //Extensions.NodeAndSegmentExtension.NodeVectorDictionary.Add(command.Position, 100); //adds a dummynode to hinder ocilliation
            //Singleton<NetManager>.instance.CreateNode(out ushort node, ref Singleton<SimulationManager>.instance.m_randomizer, netinfo, command.Position, Singleton<SimulationManager>.instance.m_currentBuildIndex);
            //Extensions.NodeAndSegmentExtension.NodeIDDictionary.Add((ushort)command.NodeId, node); //Adds the NodeID recived and the NodeID generated on NodeCreation.
            //Extensions.NodeAndSegmentExtension.NodeVectorDictionary[command.Position] = node;
            /*
            switch (MultiplayerManager.Instance.CurrentRole) //returns the newly created Nodes NodeID so that it can be added to the original builders Dictionary
            {
                case MultiplayerRole.Client:
                    {
                        Command.SendToServer(new NodeIdCommand
                        {
                            NodeIdSender = node,
                            NodeIdReciever = command.NodeId
                        });
                        break;
                    }
                case MultiplayerRole.Server:
                    {
                        Command.SendToClients(new NodeIdCommand
                        {
                            NodeIdSender = node,
                            NodeIdReciever = command.NodeId
                        });
                        break;
                    }
            }
            //Extensions.NodeAndSegmentExtension.NetSegmentLocked = false;
            */
        }
    }
}