using System;
using System.Reflection;
using CSM.Commands;
using CSM.Common;
using HarmonyLib;
using UnityEngine;

namespace CSM.Injections
{
    public static class BuildingHandler
    {
        public static bool IgnoreAll { get; set; } = false;
    }

    [HarmonyPatch]
    public class ToolCreateBuilding
    {
        public static void Prefix(out CallState __state, object __instance)
        {
            __state = new CallState();

            if (BuildingHandler.IgnoreAll)
            {
                __state.run = false;
                return;
            }

            BuildingTool tool = ReflectionHelper.GetAttr<BuildingTool>(__instance, "$this");
            int counter = ReflectionHelper.GetAttr<int>(__instance, "$PC");
            ToolBase.ToolErrors ___m_placementErrors = ReflectionHelper.GetAttr<ToolBase.ToolErrors>(tool, "m_placementErrors");

            if (counter != 0 || ___m_placementErrors != ToolBase.ToolErrors.None)
            {
                __state.run = false;
                return;
            }

            __state.run = true;
            __state.relocate = tool.m_relocate; // Save relocate state as it will be cleared at the end of the method

            NetHandler.IgnoreAll = true;
            TreeHandler.IgnoreAll = true;
            PropHandler.IgnoreAll = true;
            BuildingHandler.IgnoreAll = true;
            ArrayHandler.StartCollecting();
        }

        public static void Postfix(ref CallState __state, object __instance)
        {
            if (!__state.run)
                return;

            ArrayHandler.StopCollecting();
            BuildingHandler.IgnoreAll = false;
            PropHandler.IgnoreAll = false;
            TreeHandler.IgnoreAll = false;
            NetHandler.IgnoreAll = false;

            BuildingTool tool = ReflectionHelper.GetAttr<BuildingTool>(__instance, "$this");
            ToolController controller = ReflectionHelper.GetAttr<ToolController>(tool, "m_toolController");

            ushort prefab = 0;
            if (__state.relocate == 0)
                prefab = (ushort)Mathf.Clamp(tool.m_prefab.m_prefabDataIndex, 0, 65535);

            Vector3 mousePosition = ReflectionHelper.GetAttr<Vector3>(tool, "m_mousePosition");
            float mouseAngle = ReflectionHelper.GetAttr<float>(tool, "m_mouseAngle");
            int elevation = ReflectionHelper.GetAttr<int>(tool, "m_elevation");

            ulong[] collidingSegments = ReflectionHelper.GetAttr<ulong[]>(controller, "m_collidingSegments1");
            ulong[] collidingBuildings = ReflectionHelper.GetAttr<ulong[]>(controller, "m_collidingBuildings1");

            Command.SendToAll(new BuildingToolCreateCommand()
            {
                Array16Ids = ArrayHandler.Collected16,
                Array32Ids = ArrayHandler.Collected32,
                Prefab = prefab,
                Relocate = __state.relocate,
                CollidingSegments = collidingSegments,
                CollidingBuildings = collidingBuildings,
                MousePosition = mousePosition,
                MouseAngle = mouseAngle,
                Elevation = elevation
            });
        }

        public static MethodBase TargetMethod()
        {
            return ReflectionHelper.GetIteratorTargetMethod(typeof(BuildingTool), "<CreateBuilding>c__Iterator0", out Type _);
        }

        public class CallState
        {
            public bool run;
            public int relocate;
        }
    }

    [HarmonyPatch(typeof(BuildingManager))]
    [HarmonyPatch("CreateBuilding")]
    public class CreateBuilding
    {
        public static void Prefix(out bool __state)
        {
            if (BuildingHandler.IgnoreAll)
            {
                __state = false;
                return;
            }

            __state = true;

            NetHandler.IgnoreAll = true;
            TreeHandler.IgnoreAll = true;
            PropHandler.IgnoreAll = true;
            BuildingHandler.IgnoreAll = true;
            ArrayHandler.StartCollecting();
        }

        public static void Postfix(bool __result, ref ushort building, ref bool __state)
        {
            if (!__state)
                return;

            BuildingHandler.IgnoreAll = false;
            PropHandler.IgnoreAll = false;
            TreeHandler.IgnoreAll = false;
            NetHandler.IgnoreAll = false;
            ArrayHandler.StopCollecting();

            if (__result)
            {
                Building b = BuildingManager.instance.m_buildings.m_buffer[building];

                Command.SendToAll(new BuildingCreateCommand
                {
                    Array16Ids = ArrayHandler.Collected16,
                    Array32Ids = ArrayHandler.Collected32,
                    Position = b.m_position,
                    InfoIndex = b.m_infoIndex,
                    Angle = b.m_angle,
                    Length = b.Length
                });
            }
        }
    }

    [HarmonyPatch(typeof(BuildingManager))]
    [HarmonyPatch("ReleaseBuildingImplementation")]
    public class ReleaseBuildingImplementation
    {
        public static void Postfix(ushort building)
        {
            if (BuildingHandler.IgnoreAll)
                return;

            Command.SendToAll(new BuildingRemoveCommand
            {
                BuildingId = building
            });
        }
    }

    [HarmonyPatch(typeof(BuildingManager))]
    [HarmonyPatch("RelocateBuildingImpl")]
    public class RelocateBuildingImpl
    {
        public static void Prefix(out bool __state)
        {
            if (BuildingHandler.IgnoreAll)
            {
                __state = false;
                return;
            }

            __state = true;

            BuildingHandler.IgnoreAll = true;
        }

        public static void Postfix(ushort building, ref bool __state)
        {
            if (!__state)
                return;

            BuildingHandler.IgnoreAll = false;

            Building b = BuildingManager.instance.m_buildings.m_buffer[building];

            Command.SendToAll(new BuildingRelocateCommand
            {
                BuildingId = building,
                NewPosition = b.m_position,
                Angle = b.m_angle
            });
        }
    }
}
