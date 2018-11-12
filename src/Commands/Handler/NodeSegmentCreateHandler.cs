using ColossalFramework;
using CSM.Helpers;
using CSM.Networking;
using UnityEngine;

namespace CSM.Commands.Handler
{
    public class NodeSegmentCreateHandler : CommandHandler<NodeSegmentCreateCommand>
    {
        public override byte ID => 110;

        public override void HandleOnClient(NodeSegmentCreateCommand command) => HandleCreateSegment(command);

        public override void HandleOnServer(NodeSegmentCreateCommand command, Player player) => HandleCreateSegment(command);

        private bool HandleCreateSegment(NodeSegmentCreateCommand command)
        {

            lock (Extensions.NodeAndSegmentExtension.SegmentsCreated) { 
                NetInfo info = PrefabCollection<NetInfo>.GetPrefab(command.InfoIndex);
            Extensions.NodeAndSegmentExtension.SegmentsCreated.Add((ushort)command.SegmentID);
            uint segment = command.SegmentID;

            if (!Singleton<NetManager>.instance.m_nodes.m_buffer[command.StartNode].AddSegment((ushort)segment))
            {
                Singleton<NetManager>.instance.m_segments.ReleaseItem((ushort)segment);
                segment = 0;
                UnityEngine.Debug.Log("Segment 1 run");
                return false;
            }
            if (!Singleton<NetManager>.instance.m_nodes.m_buffer[command.EndNode].AddSegment((ushort)segment))
            {
                Singleton<NetManager>.instance.m_nodes.m_buffer[command.StartNode].RemoveSegment((ushort)segment);
                Singleton<NetManager>.instance.m_segments.ReleaseItem((ushort)segment);
                segment = 0;
                UnityEngine.Debug.Log("Segment 2 run");
                return false;
            }

            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_flags = NetSegment.Flags.Created;
            /*if (invert)
            {
                this.m_segments.m_buffer[segment].m_flags |= NetSegment.Flags.Invert;
            }
            */
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].Info = info;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_buildIndex = Singleton<SimulationManager>.instance.m_currentBuildIndex;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_modifiedIndex = command.ModifiedIndex;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_startNode = command.StartNode;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_endNode = command.EndNode;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_startDirection = command.StartDirection;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_endDirection = command.EndDirection;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_problems = Notification.Problem.None;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_blockStartLeft = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_blockStartRight = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_blockEndLeft = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_blockEndRight = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_lanes = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_path = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_trafficBuffer = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_trafficDensity = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_trafficLightState0 = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_trafficLightState1 = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_startLeftSegment = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_startRightSegment = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_endLeftSegment = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_endRightSegment = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_averageLength = 0f;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_nextGridSegment = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_cornerAngleStart = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_cornerAngleEnd = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_fireCoverage = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_wetness = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_condition = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_nameSeed = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_noiseBuffer = 0;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_noiseDensity = 0;
            info.m_netAI.CreateSegment((ushort)segment, ref Singleton<NetManager>.instance.m_segments.m_buffer[segment]);
            if (info.m_lanes != null)
            {
                Singleton<NetManager>.instance.CreateLanes(out Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_lanes, ref Singleton<SimulationManager>.instance.m_randomizer, (ushort)segment, info.m_lanes.Length);
            }
            Vector3 vector = (Singleton<NetManager>.instance.m_nodes.m_buffer[Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_startNode].m_position + Singleton<NetManager>.instance.m_nodes.m_buffer[Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_endNode].m_position) * 0.5f;
            int num = Mathf.Clamp((int)((vector.x / 64f) + 135f), 0, 0x10d);
            int index = (Mathf.Clamp((int)((vector.z / 64f) + 135f), 0, 0x10d) * 270) + num;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_nextGridSegment = Singleton<NetManager>.instance.m_segmentGrid[index];
            Singleton<NetManager>.instance.m_segmentGrid[index] = (ushort)segment;
            Singleton<NetManager>.instance.UpdateSegment((ushort)segment);
            Singleton<NetManager>.instance.UpdateSegmentColors((ushort)segment);
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].UpdateBounds((ushort)segment);
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].UpdateZones((ushort)segment);
            Singleton<NetManager>.instance.m_segmentCount = ((int)Singleton<NetManager>.instance.m_segments.ItemCount()) - 1;
            return true;
            }

        }


        //_startNode = Extensions.NodeAndSegmentExtension.NodeIDDictionary[command.StartNode]; //uses the Dictionary of the Client and Servers different NodeID to convert the recived NodeID the the NodeID of the reciever
        //_endNode = Extensions.NodeAndSegmentExtension.NodeIDDictionary[command.EndNode];
        //StartEndNode startEndNode = new StartEndNode(Singleton<NetManager>.instance.m_nodes.m_buffer[_startNode].m_position, Singleton<NetManager>.instance.m_nodes.m_buffer[_endNode].m_position);
        //Extensions.NodeAndSegmentExtension.StartEndNodeDictionary.Add(startEndNode, 100); // add dummy segment to hinder occilation
        //Singleton<NetManager>.instance.CreateSegment(out ushort id, ref Singleton<SimulationManager>.instance.m_randomizer, netinfo, _startNode, _endNode, command.StartDirection, command.EndDirection, Singleton<SimulationManager>.instance.m_currentBuildIndex, command.ModifiedIndex, false);
        //Extensions.NodeAndSegmentExtension.StartEndNodeDictionary[startEndNode] = id;
    }
}