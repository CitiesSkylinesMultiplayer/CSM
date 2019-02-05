using ColossalFramework;
using CSM.Commands;
using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSM.Injections
{
    public class NodeHandler
    {
        public static List<ushort> IgnoreSegments { get; } = new List<ushort>();
        public static List<ushort> IgnoreNodes { get; } = new List<ushort>();
    }

    [HarmonyPatch(typeof(NetManager))]
    [HarmonyPatch("CreateNode")]
    public class CreateNode
    {
        /// <summary>
        /// This handler is executed after a new NetNode is created using NetManager::CreateNode
        /// </summary>
        /// <param name="__result">The boolean return value of CreateNode states if the node was created successfully</param>
        /// <param name="node">This is the node id set by CreateNode</param>
        public static void Postfix(bool __result, ref ushort node)
        {
            UnityEngine.Debug.Log($"Create Node");
            if (__result)
            {
                NetNode netNode = Singleton<NetManager>.instance.m_nodes.m_buffer[node];
                Command.SendToAll(new NodeCreateCommand
                {
                    Position = netNode.m_position,
                    InfoIndex = netNode.m_infoIndex,
                    NodeId = node
                });
            }
        }
    }

    [HarmonyPatch(typeof(NetManager))]
    public class ReleaseNodeImpl
    {
        /// <summary>
        /// This handler is executed before a NetNode is released using NetManager::ReleaseNodeImplementation.
        /// </summary>
        /// <param name="node">The node id</param>
        /// <param name="data">The NetNode object</param>
        public static void Prefix(ushort node, ref NetNode data)
        {
            UnityEngine.Debug.Log($"Release node");
            if (data.m_flags != 0 && !NodeHandler.IgnoreNodes.Contains(node))
            {
                Command.SendToAll(new NodeReleaseCommand
                {
                    NodeId = node
                });
            }
        }

        // Get target method NetManager::ReleaseNodeImplementation(ushort, ref NetNode)
        public static MethodBase TargetMethod()
        {
            return typeof(NetManager).GetMethod("ReleaseNodeImplementation", AccessTools.all, null, new Type[] { typeof(ushort), typeof(NetNode).MakeByRefType() }, new ParameterModifier[] { });
        }
    }

    [HarmonyPatch(typeof(NetManager))]
    [HarmonyPatch("UpdateNodeFlags")]
    public class UpdateNodeFlags
    {
        /// <summary>
        /// This handler is executed after a node was refreshed in any way.
        /// </summary>
        /// <param name="node">The node id</param>
        public static void Postfix(ushort node)
        {
            UnityEngine.Debug.Log($"Update Node Flags");
            NetNode netNode = Singleton<NetManager>.instance.m_nodes.m_buffer[node];
            ushort[] segments = new ushort[8];
            segments[0] = netNode.m_segment0;
            segments[1] = netNode.m_segment1;
            segments[2] = netNode.m_segment2;
            segments[3] = netNode.m_segment3;
            segments[4] = netNode.m_segment4;
            segments[5] = netNode.m_segment5;
            segments[6] = netNode.m_segment6;
            segments[7] = netNode.m_segment7;
            Command.SendToAll(new NodeUpdateCommand
            {
                Segments = segments,
                NodeID = node,
                Flags = netNode.m_flags
            });
        }
    }

    [HarmonyPatch(typeof(NetManager))]
    [HarmonyPatch("CreateSegment")]
    public class CreateSegment
    {
        /// <summary>
        /// This handler is executed after a segment was created using NetManager::CreateSegment
        /// </summary>
        /// <param name="__result">The boolean return value of CreateSegment states if the segment was created successfully</param>
        /// <param name="segment">The segment id</param>
        public static void Postfix(bool __result, ref ushort segment)
        {
            if (__result)
            {
                UnityEngine.Debug.Log($"CreateSegment {segment}");
                NetSegment seg = Singleton<NetManager>.instance.m_segments.m_buffer[segment];
                Command.SendToAll(new SegmentCreateCommand
                {
                    SegmentID = segment,
                    StartNode = seg.m_startNode,
                    EndNode = seg.m_endNode,
                    StartDirection = seg.m_startDirection,
                    EndDirection = seg.m_endDirection,
                    ModifiedIndex = seg.m_modifiedIndex,
                    InfoIndex = seg.m_infoIndex,
                    Invert = seg.m_flags.IsFlagSet(NetSegment.Flags.Invert)
                });
            }
        }
    }

    [HarmonyPatch(typeof(NetManager))]
    public class ReleaseSegmentImpl
    {
        /// <summary>
        /// This handler is executed before a segment is released using NetManager::ReleaseSegmentImplementation
        /// </summary>
        /// <param name="segment">The segment id</param>
        /// <param name="data">The NetSegment object</param>
        /// <param name="keepNodes">If adjacent nodes should also be released</param>
        public static void Prefix(ushort segment, ref NetSegment data, bool keepNodes)
        {
            UnityEngine.Debug.Log($"Release segment");
            if (data.m_flags != 0 && !NodeHandler.IgnoreSegments.Contains(segment))
            {
                Command.SendToAll(new SegmentReleaseCommand
                {
                    SegmentId = segment,
                    KeepNodes = keepNodes
                });
            }
        }

        // Get target method NetManager::ReleaseSegmentImplementation(ushort, ref NetNode, bool)
        public static MethodBase TargetMethod()
        {
            return typeof(NetManager).GetMethod("ReleaseSegmentImplementation", AccessTools.all, null, new Type[] { typeof(ushort), typeof(NetSegment).MakeByRefType(), typeof(bool) }, new ParameterModifier[] { });
        }
    }
}
