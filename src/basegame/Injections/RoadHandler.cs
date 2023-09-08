using System;
using System.Collections.Generic;
using System.Reflection;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Roads;
using HarmonyLib;

namespace CSM.BaseGame.Injections
{
    [HarmonyPatch(typeof(RoadBaseAI))]
    [HarmonyPatch("ClickNodeButton")]
    public class ClickNodeButton
    {
        public static void Prefix(ref NetNode data, int index, out int __state)
        {
            __state = -1;

            if (IgnoreHelper.Instance.IsIgnored())
                return;

            __state = GetFlags(data, index);
        }

        public static void Postfix(ref NetNode data, int index, ushort nodeID, ref int __state)
        {
            if (__state == -1)
                return;

            int newFlags = GetFlags(data, index);
            if (newFlags != -1 && newFlags != __state)
            {
                Command.SendToAll(new RoadSettingsCommand()
                {
                    NodeId = nodeID,
                    Index = index
                });
            }
        }

        private static int GetFlags(NetNode data, int index)
        {
            if (index == -1) // Node modified (traffic lights)
            {
                return (int)data.m_flags;
            }
            else // Segment modified (stop sign)
            {
                ushort segment = data.GetSegment(index - 1);
                if (segment != 0)
                {
                    return (int)NetManager.instance.m_segments.m_buffer[segment].m_flags;
                }
            }

            return -1;
        }
    }

    [HarmonyPatch]
    public class SetPriorityRoad
    {
        public static void Prefix(object __instance, out bool __state)
        {
            int counter = ReflectionHelper.GetAttr<int>(__instance, "$PC");
            __state = counter == 0;
        }

        public static void Postfix(IEnumerator<bool> __instance, ref bool __state, ushort ___segmentID, bool ___priority)
        {
            if (IgnoreHelper.Instance.IsIgnored() || !__state)
                return;

            if (!ReflectionHelper.GetAttr<bool>(__instance, "$current"))
                return;

            Command.SendToAll(new RoadSetPriorityCommand
            {
                SegmentId = ___segmentID,
                Priority = ___priority
            });
        }

        public static MethodBase TargetMethod()
        {
            return ReflectionHelper.GetIteratorTargetMethod(typeof(NetManager), "<SetPriorityRoad>c__Iterator3", out Type _);
        }
    }

    [HarmonyPatch(typeof(NetAdjust))]
    [HarmonyPatch("ApplyModification")]
    public class ApplyModification
    {
        public static void Prefix(int index, bool ___m_pathVisible, int ___m_tempAdjustmentIndex,
                HashSet<ushort> ___m_originalSegments, HashSet<ushort> ___m_includedSegments,
                InstanceID ___m_lastInstance)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            if (!___m_pathVisible || ___m_tempAdjustmentIndex != index)
                return;

            Command.SendToAll(new RoadAdjustCommand()
            {
                OriginalSegments = ___m_originalSegments,
                IncludedSegments = ___m_includedSegments,
                LastInstance = ___m_lastInstance,
            });
        }
    }
}
