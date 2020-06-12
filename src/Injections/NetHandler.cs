using CSM.Commands;
using CSM.Commands.Data.Net;
using CSM.Helpers;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace CSM.Injections
{
    [HarmonyPatch]
    public class ToolCreateNode
    {
        public static void Prefix(out CallState __state, bool test, bool visualize,
            NetTool.ControlPoint startPoint, NetTool.ControlPoint middlePoint, NetTool.ControlPoint endPoint)
        {
            __state = new CallState();

            if (IgnoreHelper.IsIgnored())
            {
                __state.valid = false;
                return;
            }

            if (test || visualize)
            {
                __state.valid = false;
                return;
            }

            __state.valid = true;
            __state.SetControlPoints(startPoint, middlePoint, endPoint);

            IgnoreHelper.StartIgnore();
            ArrayHandler.StartCollecting();
        }

        public static void Postfix(NetInfo info, int maxSegments, bool testEnds,
            bool autoFix, bool invert, bool switchDir, ushort relocateBuildingID, ref CallState __state)
        {
            if (!__state.valid)
                return;

            ArrayHandler.StopCollecting();
            IgnoreHelper.EndIgnore();

            ushort prefab = (ushort)Mathf.Clamp(info.m_prefabDataIndex, 0, 65535);

            Command.SendToAll(new NodeCreateCommand()
            {
                Array16Ids = ArrayHandler.Collected16,
                Array32Ids = ArrayHandler.Collected32,
                Prefab = prefab,
                StartPoint = __state.start,
                MiddlePoint = __state.middle,
                EndPoint = __state.end,
                MaxSegments = maxSegments,
                TestEnds = testEnds,
                AutoFix = autoFix,
                Invert = invert,
                SwitchDir = switchDir,
                RelocateBuildingId = relocateBuildingID
            });
        }

        public static MethodBase TargetMethod()
        {
            return typeof(NetTool).GetMethod("CreateNode", new Type[]
            {
                typeof(NetInfo), typeof(NetTool.ControlPoint), typeof(NetTool.ControlPoint),
                typeof(NetTool.ControlPoint),
                typeof(FastList<>).MakeGenericType(typeof(NetTool.NodePosition)), typeof(int),
                typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool),
                typeof(bool), typeof(ushort), typeof(ushort).MakeByRefType(), typeof(ushort).MakeByRefType(),
                typeof(ushort).MakeByRefType(), typeof(int).MakeByRefType(), typeof(int).MakeByRefType()
            });
        }

        public class CallState
        {
            public bool valid;
            public NetTool.ControlPoint start, middle, end;

            public void SetControlPoints(NetTool.ControlPoint startPoint, NetTool.ControlPoint middlePoint,
                NetTool.ControlPoint endPoint)
            {
                start = CopyPoint(startPoint);
                middle = CopyPoint(middlePoint);
                end = CopyPoint(endPoint);
            }

            private NetTool.ControlPoint CopyPoint(NetTool.ControlPoint point)
            {
                return new NetTool.ControlPoint()
                {
                    m_direction = point.m_direction,
                    m_elevation = point.m_elevation,
                    m_node = point.m_node,
                    m_outside = point.m_outside,
                    m_position = point.m_position,
                    m_segment = point.m_segment
                };
            }
        }
    }

    /*
     * Update building creates buildings like for example power poles, this should not be synced as it happens
     * on both sides equally! (Otherwise we would have double buildings)
     */

    [HarmonyPatch(typeof(NetNode))]
    [HarmonyPatch("UpdateBuilding")]
    public class UpdateBuilding
    {
        public static void Prefix()
        {
            IgnoreHelper.StartIgnore();
        }

        public static void Postfix()
        {
            IgnoreHelper.EndIgnore();
        }
    }

    [HarmonyPatch]
    public class ReleaseNodeImpl
    {
        /// <summary>
        /// This handler is executed before a NetNode is released using NetManager::ReleaseNodeImplementation.
        /// </summary>
        /// <param name="node">The node id</param>
        /// <param name="data">The NetNode object</param>
        public static void Prefix(ushort node, ref NetNode data)
        {
            if (IgnoreHelper.IsIgnored())
                return;

            if (data.m_flags != 0)
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
            return typeof(NetManager).GetMethod("ReleaseNodeImplementation", ReflectionHelper.AllAccessFlags, null,
                new Type[] { typeof(ushort), typeof(NetNode).MakeByRefType() }, null);
        }
    }

    [HarmonyPatch]
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
            if (IgnoreHelper.IsIgnored())
                return;

            if (data.m_flags != 0)
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
            return typeof(NetManager).GetMethod("ReleaseSegmentImplementation", ReflectionHelper.AllAccessFlags, null,
                new Type[] { typeof(ushort), typeof(NetSegment).MakeByRefType(), typeof(bool) }, null);
        }
    }
}
