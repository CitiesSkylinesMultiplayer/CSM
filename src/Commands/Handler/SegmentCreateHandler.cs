using ColossalFramework;
using CSM.Helpers;
using CSM.Networking;
using Harmony;
using System.Reflection;

namespace CSM.Commands.Handler
{
    public class SegmentCreateHandler : CommandHandler<SegmentCreateCommand>
    {
        private MethodInfo _initializeSegment;

        public SegmentCreateHandler()
        {
            _initializeSegment = typeof(NetManager).GetMethod("InitializeSegment", AccessTools.all);
        }

        public override byte ID => 110;

        public override void HandleOnClient(SegmentCreateCommand command) => HandleCreateSegment(command);

        public override void HandleOnServer(SegmentCreateCommand command, Player player) => HandleCreateSegment(command);

        private void HandleCreateSegment(SegmentCreateCommand command)
        {
            NetInfo info = PrefabCollection<NetInfo>.GetPrefab(command.InfoIndex);
            ushort segment = command.SegmentID;

            Singleton<NetManager>.instance.m_segments.RemoveUnused(segment);

            if (!Singleton<NetManager>.instance.m_nodes.m_buffer[command.StartNode].AddSegment(segment))
            {
                Singleton<NetManager>.instance.m_segments.ReleaseItem(segment);
            }
            if (!Singleton<NetManager>.instance.m_nodes.m_buffer[command.EndNode].AddSegment(segment))
            {
                Singleton<NetManager>.instance.m_nodes.m_buffer[command.StartNode].RemoveSegment(segment);
                Singleton<NetManager>.instance.m_segments.ReleaseItem(segment);
            }

            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_flags = NetSegment.Flags.Created;
            if (command.Invert)
            {
                Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_flags |= NetSegment.Flags.Invert;
            }
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
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_lanes = 0u;
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_path = 0u;
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

            info.m_netAI.CreateSegment(segment, ref Singleton<NetManager>.instance.m_segments.m_buffer[segment]);
            if (info.m_lanes != null)
            {
                Singleton<NetManager>.instance.CreateLanes(out Singleton<NetManager>.instance.m_segments.m_buffer[segment].m_lanes, ref Singleton<SimulationManager>.instance.m_randomizer, segment, info.m_lanes.Length);
            }

            _initializeSegment.Invoke(Singleton<NetManager>.instance, new object[] { segment, Singleton<NetManager>.instance.m_segments.m_buffer[segment] });

            Singleton<NetManager>.instance.UpdateSegment(segment);
            Singleton<NetManager>.instance.UpdateSegmentColors(segment);
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].UpdateBounds(segment);
            Singleton<NetManager>.instance.m_segments.m_buffer[segment].UpdateZones(segment);
            Singleton<NetManager>.instance.m_segmentCount = ((int)Singleton<NetManager>.instance.m_segments.ItemCount()) - 1;

            Singleton<SimulationManager>.instance.m_currentBuildIndex++;
        }
    }
}