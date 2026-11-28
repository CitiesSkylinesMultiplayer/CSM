using System;
using System.Collections.Generic;
using System.Reflection;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Buildings;
using CSM.BaseGame.Helpers;
using HarmonyLib;

namespace CSM.BaseGame.Injections
{
    [HarmonyPatch]
    public class BuildingSimulationStatusPatch
    {
        public struct Scope
        {
            public bool Client;
            public bool AbandonmentDisabled;
        }

        public static IEnumerable<MethodBase> TargetMethods()
        {
            Type[] signature = { typeof(ushort), typeof(Building).MakeByRefType() };
            foreach (Type type in typeof(BuildingAI).Assembly.GetTypes())
            {
                if (!typeof(BuildingAI).IsAssignableFrom(type))
                    continue;
                MethodInfo method = type.GetMethod("SimulationStep", BindingFlags.Instance | BindingFlags.Public |
                    BindingFlags.NonPublic | BindingFlags.DeclaredOnly, null, signature, null);
                if (method != null && !method.IsAbstract)
                    yield return method;
            }
        }

        public static void Prefix(ushort __0, ref Building __1, out Scope __state)
        {
            __state = default(Scope);
            if (Command.CurrentRole != MultiplayerRole.Client)
                return;
            __state.Client = true;
            __state.AbandonmentDisabled = BuildingManager.instance.m_abandonmentDisabled;
            BuildingManager.instance.m_abandonmentDisabled = true;
            BuildingStatusSync.EnterClientSimulation();
            BuildingStatusSync.Enforce(__0, ref __1);
        }

        public static Exception Finalizer(ushort __0, ref Building __1, Scope __state, Exception __exception)
        {
            if (__state.Client)
            {
                try
                {
                    BuildingStatusSync.Enforce(__0, ref __1);
                }
                finally
                {
                    BuildingManager.instance.m_abandonmentDisabled = __state.AbandonmentDisabled;
                    BuildingStatusSync.ExitClientSimulation();
                }
            }
            else if (__exception == null)
                BuildingStatusSync.MarkDirty(__0);
            return __exception;
        }
    }

    [HarmonyPatch(typeof(CommonBuildingAI), "HandleFire")]
    public class HostFireSimulationPatch
    {
        public static bool Prefix(ushort buildingID, ref Building data, ref Building.Frame frameData)
        {
            if (Command.CurrentRole != MultiplayerRole.Client)
                return true;
            BuildingStatusState state = BuildingStatusSync.GetClientState(buildingID, ref data);
            if (state != null)
            {
                frameData.m_fireDamage = state.FireDamage;
                if ((state.Conditions & (uint)Building.Flags.Collapsed) != 0)
                    frameData.m_constructState = state.ConstructState;
            }
            return false;
        }

        public static void Postfix(ushort buildingID, ref Building data)
        {
            if (data.m_fireIntensity != 0 || data.GetLastFrameData().m_fireDamage != 0)
                BuildingStatusSync.MarkFamilyDirty(buildingID);
        }
    }

    [HarmonyPatch(typeof(CommonBuildingAI), "BurnBuilding")]
    public class HostIgnitionPatch
    {
        public static bool Prefix(bool testOnly, ref bool __result)
        {
            if (Command.CurrentRole != MultiplayerRole.Client || testOnly)
                return true;
            __result = false;
            return false;
        }

        public static void Postfix(ushort buildingID, bool testOnly, bool __result)
        {
            if (__result && !testOnly)
                BuildingStatusSync.MarkFamilyDirty(buildingID);
        }
    }

    [HarmonyPatch]
    public class HostExtinguishingPatch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            Type[] signature = { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(ushort), typeof(Building).MakeByRefType() };
            yield return AccessTools.DeclaredMethod(typeof(FireTruckAI), "ExtinguishFire", signature);
            yield return AccessTools.DeclaredMethod(typeof(FireCopterAI), "ExtinguishFire", signature);
        }

        public static bool Prefix(ref Building buildingData, ref bool __result)
        {
            if (Command.CurrentRole != MultiplayerRole.Client)
                return true;
            // Let local vehicles finish their task when the host ends the fire.
            __result = buildingData.m_fireIntensity == 0;
            return false;
        }

        public static void Postfix(ushort buildingID)
        {
            BuildingStatusSync.MarkFamilyDirty(buildingID);
        }
    }

    [HarmonyPatch(typeof(CommonBuildingAI), "HandleSick")]
    public class HostBuildingSicknessPatch
    {
        public static bool Prefix()
        {
            return Command.CurrentRole != MultiplayerRole.Client;
        }
    }

    [HarmonyPatch(typeof(CommonBuildingAI), "GetHomeBehaviour")]
    public class HostHomeHealthPatch
    {
        public static void Postfix(ushort buildingID, ref Building buildingData, ref Citizen.BehaviourData behaviour, int aliveCount)
        {
            BuildingStatusState state = BuildingStatusSync.GetClientState(buildingID, ref buildingData);
            if (state != null)
                state.ApplyHomeHealth(ref behaviour, aliveCount);
        }
    }

    [HarmonyPatch(typeof(BuildingManager), "ReleaseBuilding", new[] { typeof(ushort) })]
    public class HostSimulationReleasePatch
    {
        public static bool AllowSimulationRelease(Building.Flags flags)
        {
            // CommonBuildingAI and DecorationBuildingAI finish an explicit demolition in their simulation wrapper.
            return (flags & Building.Flags.Demolishing) != 0;
        }

        public static bool Prefix(BuildingManager __instance, ushort building)
        {
            if (Command.CurrentRole != MultiplayerRole.Client || !BuildingStatusSync.InClientSimulation)
                return true;
            return building != 0 && building < __instance.m_buildings.m_buffer.Length &&
                AllowSimulationRelease(__instance.m_buildings.m_buffer[building].m_flags);
        }
    }

    [HarmonyPatch(typeof(CommonBuildingAI), "CollapseBuilding")]
    public class HostSimulationCollapsePatch
    {
        public static bool Prefix(bool testOnly, ref bool __result)
        {
            if (Command.CurrentRole != MultiplayerRole.Client || !BuildingStatusSync.InClientSimulation || testOnly)
                return true;
            __result = false;
            return false;
        }

        public static void Postfix(ushort buildingID, bool testOnly, bool __result)
        {
            if (__result && !testOnly)
                BuildingStatusSync.MarkFamilyDirty(buildingID);
        }
    }

    [HarmonyPatch(typeof(BuildingManager), "CreateBuilding")]
    public class BuildingStatusCreatePatch
    {
        public static void Postfix(bool __result, ushort building)
        {
            if (__result)
                BuildingStatusSync.Invalidate(building);
        }
    }

    [HarmonyPatch(typeof(BuildingManager), "ReleaseBuildingImplementation")]
    public class BuildingStatusReleasePatch
    {
        public static void Prefix(ushort building)
        {
            BuildingStatusSync.Invalidate(building);
        }
    }

    [HarmonyPatch(typeof(BuildingManager), "RelocateBuildingImpl")]
    public class BuildingStatusRelocatePatch
    {
        public static void Postfix(ushort building)
        {
            BuildingStatusSync.Invalidate(building);
        }
    }

    [HarmonyPatch(typeof(BuildingManager), "UpdateBuildingInfo", new[] { typeof(ushort), typeof(BuildingInfo) })]
    public class BuildingStatusInfoPatch
    {
        public static void Postfix(ushort building)
        {
            BuildingStatusSync.Invalidate(building);
        }
    }

    [HarmonyPatch]
    public class BuildingStatusRebuildPatch
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            return OnRebuildClicked.TargetMethods();
        }

        public static void Postfix(object __instance)
        {
            BuildingStatusSync.Invalidate(ReflectionHelper.GetAttr<ushort>(__instance, "buildingID"));
        }
    }
}
