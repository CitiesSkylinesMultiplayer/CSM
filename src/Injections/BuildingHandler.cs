using CSM.Commands;
using CSM.Commands.Data.Buildings;
using CSM.Helpers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CSM.Injections
{
    [HarmonyPatch]
    public class ToolCreateBuilding
    {
        public static void Prefix(out CallState __state, object __instance)
        {
            __state = new CallState();

            if (IgnoreHelper.IsIgnored())
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

            IgnoreHelper.StartIgnore();
            ArrayHandler.StartCollecting();
        }

        public static void Postfix(ref CallState __state, object __instance)
        {
            if (!__state.run)
                return;

            ArrayHandler.StopCollecting();
            IgnoreHelper.EndIgnore();

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
            if (IgnoreHelper.IsIgnored())
            {
                __state = false;
                return;
            }

            __state = true;

            IgnoreHelper.StartIgnore();
            ArrayHandler.StartCollecting();
        }

        public static void Postfix(bool __result, ref ushort building, ref bool __state)
        {
            if (!__state)
                return;

            IgnoreHelper.EndIgnore();
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
            if (IgnoreHelper.IsIgnored())
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
            if (IgnoreHelper.IsIgnored())
            {
                __state = false;
                return;
            }

            __state = true;

            IgnoreHelper.StartIgnore();
        }

        public static void Postfix(ushort building, ref bool __state)
        {
            if (!__state)
                return;

            IgnoreHelper.EndIgnore();

            Building b = BuildingManager.instance.m_buildings.m_buffer[building];

            Command.SendToAll(new BuildingRelocateCommand
            {
                BuildingId = building,
                NewPosition = b.m_position,
                Angle = b.m_angle
            });
        }
    }

    [HarmonyPatch]
    public class SetProductionRate
    {
        public static void Prefix(ushort buildingID, byte rate, ref Building data)
        {
            if (IgnoreHelper.IsIgnored())
                return;

            if (rate == data.m_productionRate)
                return;

            Command.SendToAll(new BuildingChangeProductionRateCommand()
            {
                Building = buildingID,
                Rate = rate
            });
        }

        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return typeof(PlayerBuildingAI).GetMethod("SetProductionRate");
            yield return typeof(ShelterAI).GetMethod("SetProductionRate");
        }
    }

    [HarmonyPatch]
    public class SetEmptying
    {
        public static void Prefix(ushort buildingID, ref Building data, bool emptying)
        {
            if (IgnoreHelper.IsIgnored())
                return;

            bool isEmptying = (data.m_flags & Building.Flags.Downgrading) != Building.Flags.None;
            if (isEmptying == emptying)
                return;

            Command.SendToAll(new BuildingSetEmptyingFillingCommand()
            {
                Building = buildingID,
                Value = emptying,
                SetEmptying = true
            });
        }

        public static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (Type t in new Type[] { typeof(CemeteryAI), typeof(LandfillSiteAI), typeof(ShelterAI), typeof(SnowDumpAI), typeof(WarehouseAI), typeof(TransportStationAI) })
            {
                yield return t.GetMethod("SetEmptying");
            }
        }
    }

    [HarmonyPatch(typeof(WarehouseAI))]
    [HarmonyPatch("SetFilling")]
    public class SetFilling
    {
        public static void Prefix(ushort buildingID, ref Building data, bool filling)
        {
            if (IgnoreHelper.IsIgnored())
                return;

            bool isFilling = (data.m_flags & Building.Flags.Filling) != Building.Flags.None;
            if (isFilling == filling)
                return;

            Command.SendToAll(new BuildingSetEmptyingFillingCommand()
            {
                Building = buildingID,
                Value = filling,
                SetEmptying = false
            });
        }
    }

    [HarmonyPatch(typeof(TollBoothAI))]
    [HarmonyPatch("SetTollPrice")]
    public class SetToolPrice
    {
        public static void Prefix(ushort buildingID, ref Building data, int price, TollBoothAI __instance)
        {
            if (IgnoreHelper.IsIgnored())
                return;

            if (__instance.GetTollPrice(buildingID, ref data) == price)
                return;

            Command.SendToAll(new BuildingSetTollPriceCommand()
            {
                Building = buildingID,
                Price = price
            });
        }
    }

    [HarmonyPatch]
    public class OnRebuildClicked
    {
        public static void Prefix(out bool __state)
        {
            if (IgnoreHelper.IsIgnored())
            {
                __state = false;
                return;
            }

            __state = true;
            ArrayHandler.StartCollecting();
            IgnoreHelper.StartIgnore();
        }

        public static void Postfix(object __instance, ref bool __state)
        {
            if (!__state)
                return;

            IgnoreHelper.EndIgnore();
            ArrayHandler.StopCollecting();

            ushort building = ReflectionHelper.GetAttr<ushort>(__instance, "buildingID");

            Command.SendToAll(new BuildingRebuildCommand()
            {
                Building = building,
                Array16Ids = ArrayHandler.Collected16
            });
        }

        public static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (Type t in new Type[] {typeof (CityServiceWorldInfoPanel), typeof(EventBuildingWorldInfoPanel),
                typeof(UniqueFactoryWorldInfoPanel), typeof(WarehouseWorldInfoPanel)})
            {
                // See decompiled code with compiler generated classes
                int anonStoreId = (t == typeof(CityServiceWorldInfoPanel)) ? 6 : 2;
                Type delegateHandler = t.GetNestedType("<OnRebuildClicked>c__AnonStorey" + anonStoreId, ReflectionHelper.AllAccessFlags);
                yield return delegateHandler.GetMethod("<>m__0", ReflectionHelper.AllAccessFlags);
            }
        }
    }

    [HarmonyPatch(typeof(BuildingManager))]
    [HarmonyPatch("UpgradeBuilding")]
    [HarmonyPatch(new Type[] { typeof(ushort), typeof(bool) })]
    public class UpgradeBuilding
    {
        public static void Prefix(out bool __state)
        {
            if (IgnoreHelper.IsIgnored())
            {
                __state = false;
                return;
            }

            __state = true;
            ArrayHandler.StartCollecting();
            IgnoreHelper.StartIgnore();
        }

        public static void Postfix(ushort buildingID, ref bool __state)
        {
            if (!__state)
                return;

            IgnoreHelper.EndIgnore();
            ArrayHandler.StopCollecting();

            Command.SendToAll(new BuildingUpgradeCommand()
            {
                Array16Ids = ArrayHandler.Collected16,
                Array32Ids = ArrayHandler.Collected32,
                Building = buildingID
            });
        }
    }

    [HarmonyPatch(typeof(WarehouseAI))]
    [HarmonyPatch("SetTransferReason")]
    public class SetTransferReason
    {
        public static void Prefix(ushort buildingID, ref Building data, TransferManager.TransferReason material, WarehouseAI __instance)
        {
            if (IgnoreHelper.IsIgnored())
                return;

            if (__instance.m_storageType != TransferManager.TransferReason.None ||
                __instance.GetTransferReason(buildingID, ref data) == material)
                return;

            Command.SendToAll(new BuildingSetTransferReasonCommand()
            {
                Building = buildingID,
                Material = material
            });
        }
    }

    [HarmonyPatch(typeof(PrivateBuildingAI))]
    [HarmonyPatch("SetHistorical")]
    public class SetHistorical
    {
        public static void Prefix(ushort buildingID, ref Building data, bool historical, BuildingAI __instance)
        {
            if (IgnoreHelper.IsIgnored())
                return;

            if (__instance.IsHistorical(buildingID, ref data, out bool _) == historical)
                return;

            Command.SendToAll(new BuildingSetHistoricalCommand()
            {
                Building = buildingID,
                Historical = historical
            });
        }
    }
}
