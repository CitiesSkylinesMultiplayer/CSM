using CSM.Commands;
using System;
using System.Reflection;
using CSM.Commands.Data.TransportLines;
using CSM.Helpers;
using HarmonyLib;
using UnityEngine;

namespace CSM.Injections
{
    public static class TransportHandler
    {
        public static bool TrackSimulationStep { get; set; } = false;
        public static ushort TrackTempLine { get; set; }
        public static bool DidUpdateLinesNow { get; set; } = false;
        public static bool DidUpdatePaths { get; set; } = false;
    }

    [HarmonyPatch]
    public class NewLine
    {
        public static void Prefix(object __instance, ref bool __state)
        {
            if (IgnoreHelper.IsIgnored())
                return;

            TransportTool tool = ReflectionHelper.GetAttr<TransportTool>(__instance, "$this");
            int counter = ReflectionHelper.GetAttr<int>(__instance, "$PC");

            int mode = ReflectionHelper.GetAttr<int>(tool, "m_mode");
            int expectedMode = ReflectionHelper.GetEnumValue(typeof(TransportTool).GetNestedType("Mode", ReflectionHelper.AllAccessFlags), "NewLine");

            ToolBase.ToolErrors m_errors = ReflectionHelper.GetAttr<ToolBase.ToolErrors>(tool, "m_errors");

            if (counter != 0 || m_errors != ToolBase.ToolErrors.None || mode != expectedMode)
            {
                __state = false;
                return;
            }
            __state = true;

            ArrayHandler.StartCollecting();
            IgnoreHelper.StartIgnore("NewLine");
        }

        public static void Postfix(object __instance, ref bool __state)
        {
            if (IgnoreHelper.IsIgnored("NewLine"))
                return;

            if (!__state)
            {
                return;
            }
            
            IgnoreHelper.EndIgnore("NewLine");
            ArrayHandler.StopCollecting();

            TransportTool tool = ReflectionHelper.GetAttr<TransportTool>(__instance, "$this");

            ushort prefab = (ushort) Mathf.Clamp(tool.m_prefab.m_prefabDataIndex, 0, 65535);
            int building = tool.m_building;

            Command.SendToAll(new TransportLineCreateCommand()
            {
                Array16Ids = ArrayHandler.Collected16,
                Prefab = prefab,
                Building = building
            });
        }

        public static MethodBase TargetMethod()
        {
            return ReflectionHelper.GetIteratorTargetMethod(typeof(TransportTool), "<NewLine>c__Iterator0", out Type _);
        }
    }

    [HarmonyPatch]
    public class RemoveStop
    {
        public static void Prefix(object __instance, ref bool __state)
        {
            if (IgnoreHelper.IsIgnored())
                return;

            TransportTool tool = ReflectionHelper.GetAttr<TransportTool>(__instance, "$this");
            int counter = ReflectionHelper.GetAttr<int>(__instance, "$PC");

            int mode = ReflectionHelper.GetAttr<int>(tool, "m_mode");
            int expectedMode = ReflectionHelper.GetEnumValue(typeof(TransportTool).GetNestedType("Mode", ReflectionHelper.AllAccessFlags), "NewLine");

            ToolBase.ToolErrors m_errors = ReflectionHelper.GetAttr<ToolBase.ToolErrors>(tool, "m_errors");
            ushort m_lastEditLine = ReflectionHelper.GetAttr<ushort>(tool, "m_lastEditLine");
            int m_hoverStopIndex = ReflectionHelper.GetAttr<int>(tool, "m_hoverStopIndex");
            int m_building = ReflectionHelper.GetAttr<int>(tool, "m_building");

            if (counter != 0 || m_errors != ToolBase.ToolErrors.None || mode != expectedMode || m_lastEditLine == 0 || m_hoverStopIndex == -1 || (m_building != 0 && m_hoverStopIndex == 0))
            {
                __state = false;
                return;
            }
            __state = true;

            ArrayHandler.StartCollecting();
            IgnoreHelper.StartIgnore("RemoveStop");
        }

        public static void Postfix(object __instance, ref bool __state)
        {
            if (IgnoreHelper.IsIgnored("RemoveStop"))
                return;

            if (!__state)
            {
                return;
            }

            TransportTool tool = ReflectionHelper.GetAttr<TransportTool>(__instance, "$this");

            IgnoreHelper.EndIgnore("RemoveStop");
            ArrayHandler.StopCollecting();

            int building = ReflectionHelper.GetAttr<int>(tool, "m_building");
            ushort prefab = (ushort) Mathf.Clamp(tool.m_prefab.m_prefabDataIndex, 0, 65535);

            Command.SendToAll(new TransportLineRemoveStopCommand()
            {
                Array16Ids = ArrayHandler.Collected16,
                Building = building,
                Prefab = prefab
            });
        }

        public static MethodBase TargetMethod()
        {
            return ReflectionHelper.GetIteratorTargetMethod(typeof(TransportTool), "<RemoveStop>c__Iterator1", out Type _);
        }
    }

    [HarmonyPatch]
    public class AddStop
    {
        public static void Prefix(object __instance, ref bool __state)
        {
            if (IgnoreHelper.IsIgnored())
                return;

            TransportTool tool = ReflectionHelper.GetAttr<TransportTool>(__instance, "$this");
            int counter = ReflectionHelper.GetAttr<int>(__instance, "$PC");

            int mode = ReflectionHelper.GetAttr<int>(tool, "m_mode");
            int expectedMode = ReflectionHelper.GetEnumValue(typeof(TransportTool).GetNestedType("Mode", ReflectionHelper.AllAccessFlags), "AddStops");

            ToolBase.ToolErrors m_errors = ReflectionHelper.GetAttr<ToolBase.ToolErrors>(tool, "m_errors");
            ushort m_line = ReflectionHelper.GetAttr<ushort>(tool, "m_line");
            int m_lastAddIndex = ReflectionHelper.GetAttr<int>(tool, "m_lastAddIndex");

            if (counter != 0 || m_errors != ToolBase.ToolErrors.None || mode != expectedMode || m_line == 0 || m_lastAddIndex == -2)
            {
                __state = false;
                return;
            }
            __state = true;

            ArrayHandler.StartCollecting();
            IgnoreHelper.StartIgnore("AddStop");
        }

        public static void Postfix(ref bool __state)
        {
            if (IgnoreHelper.IsIgnored("AddStop"))
                return;

            if (!__state)
            {
                return;
            }

            IgnoreHelper.EndIgnore("AddStop");
            ArrayHandler.StopCollecting();

            Command.SendToAll(new TransportLineAddStopCommand()
            {
                Array16Ids = ArrayHandler.Collected16
            });
        }

        public static MethodBase TargetMethod()
        {
            return ReflectionHelper.GetIteratorTargetMethod(typeof(TransportTool), "<AddStop>c__Iterator2", out Type _);
        }
    }
    
    [HarmonyPatch]
    public class MoveStop
    {
        public static void Prefix(object __instance, ref bool __state)
        {
            if (IgnoreHelper.IsIgnored())
                return;

            TransportTool tool = ReflectionHelper.GetAttr<TransportTool>(__instance, "$this");
            int counter = ReflectionHelper.GetAttr<int>(__instance, "$PC");

            int mode = ReflectionHelper.GetAttr<int>(tool, "m_mode");
            int expectedMode = ReflectionHelper.GetEnumValue(typeof(TransportTool).GetNestedType("Mode", ReflectionHelper.AllAccessFlags), "MoveStops");

            if (counter != 0 || mode != expectedMode)
            {
                __state = false;
                return;
            }
            __state = true;

            ArrayHandler.StartCollecting();
            IgnoreHelper.StartIgnore("MoveStop");
        }

        public static void Postfix(ref bool __state, bool ___applyChanges)
        {
            if (IgnoreHelper.IsIgnored("MoveStop"))
                return;

            if (!__state)
            {
                return;
            }

            IgnoreHelper.EndIgnore("MoveStop");
            ArrayHandler.StopCollecting();

            Command.SendToAll(new TransportLineMoveStopCommand()
            {
                Array16Ids = ArrayHandler.Collected16,
                ApplyChanges = ___applyChanges
            });
        }

        public static MethodBase TargetMethod()
        {
            return ReflectionHelper.GetIteratorTargetMethod(typeof(TransportTool), "<MoveStop>c__Iterator3", out Type _);
        }
    }
    
    [HarmonyPatch]
    public class CancelPrevStop
    {
        public static void Prefix(object __instance, ref bool __state)
        {
            if (IgnoreHelper.IsIgnored())
                return;

            TransportTool tool = ReflectionHelper.GetAttr<TransportTool>(__instance, "$this");
            int counter = ReflectionHelper.GetAttr<int>(__instance, "$PC");

            int mode = ReflectionHelper.GetAttr<int>(tool, "m_mode");
            int expectedMode = ReflectionHelper.GetEnumValue(typeof(TransportTool).GetNestedType("Mode", ReflectionHelper.AllAccessFlags), "AddStops");
            ushort line = ReflectionHelper.GetAttr<ushort>(tool, "m_line");

            if (counter != 0 || mode != expectedMode || line == 0)
            {
                __state = false;
                return;
            }
            __state = true;

            ArrayHandler.StartCollecting();
            IgnoreHelper.StartIgnore("CancelPrevStop");
        }

        public static void Postfix(object __instance, ref bool __state)
        {
            if (IgnoreHelper.IsIgnored("CancelPrevStop"))
                return;

            if (!__state)
            {
                return;
            }

            IgnoreHelper.EndIgnore("CancelPrevStop");
            ArrayHandler.StopCollecting();

            TransportTool tool = ReflectionHelper.GetAttr<TransportTool>(__instance, "$this");
            ushort prefab = (ushort) Mathf.Clamp(tool.m_prefab.m_prefabDataIndex, 0, 65535);
            int building = tool.m_building;

            Command.SendToAll(new TransportLineCancelPrevStopCommand()
            {
                Array16Ids = ArrayHandler.Collected16,
                Prefab = prefab,
                Building = building
            });
        }

        public static MethodBase TargetMethod()
        {
            return ReflectionHelper.GetIteratorTargetMethod(typeof(TransportTool), "<CancelPrevStop>c__Iterator4", out Type _);
        }
    }

    [HarmonyPatch]
    public class CancelMoveStop
    {
        public static void Prefix(object __instance, ref bool __state)
        {
            if (IgnoreHelper.IsIgnored())
                return;

            TransportTool tool = ReflectionHelper.GetAttr<TransportTool>(__instance, "$this");
            int counter = ReflectionHelper.GetAttr<int>(__instance, "$PC");

            int mode = ReflectionHelper.GetAttr<int>(tool, "m_mode");
            int expectedMode = ReflectionHelper.GetEnumValue(
                    typeof(TransportTool).GetNestedType("Mode", ReflectionHelper.AllAccessFlags), "MoveStops");

            if (counter != 0 || mode != expectedMode)
            {
                __state = false;
                return;
            }

            __state = true;

            ArrayHandler.StartCollecting();
            IgnoreHelper.StartIgnore("CancelMoveStop");
        }

        public static void Postfix(object __instance, ref bool __state)
        {
            if (IgnoreHelper.IsIgnored("CancelMoveStop"))
                return;

            if (!__state)
            {
                return;
            }

            IgnoreHelper.EndIgnore("CancelMoveStop");
            ArrayHandler.StopCollecting();

            TransportTool tool = ReflectionHelper.GetAttr<TransportTool>(__instance, "$this");
            ushort prefab = (ushort) Mathf.Clamp(tool.m_prefab.m_prefabDataIndex, 0, 65535);

            Command.SendToAll(new TransportLineCancelMoveStopCommand()
            {
                Array16Ids = ArrayHandler.Collected16,
                Prefab = prefab
            });
        }

        public static MethodBase TargetMethod()
        {
            return ReflectionHelper.GetIteratorTargetMethod(typeof(TransportTool), "<CancelMoveStop>c__Iterator5", out Type _);
        }
    }

    [HarmonyPatch(typeof(TransportTool))]
    [HarmonyPatch("OnEnable")]
    public class OnEnable
    {
        public static void Postfix()
        {
            if (IgnoreHelper.IsIgnored())
                return;
            
            Command.SendToAll(new TransportLineInitCommand());
        }
    }

    [HarmonyPatch(typeof(TransportTool))]
    [HarmonyPatch("ResetTool")]
    public class ResetTool
    {
        public static void Prefix()
        {
            if (IgnoreHelper.IsIgnored())
                return;
            
            ArrayHandler.StartCollecting();
            IgnoreHelper.StartIgnore("ResetTool");
        }
        
        public static void Postfix()
        {
            if (IgnoreHelper.IsIgnored("ResetTool"))
                return;
            
            IgnoreHelper.EndIgnore("ResetTool");
            ArrayHandler.StopCollecting();

            Command.SendToAll(new TransportLineResetCommand()
            {
                Array16Ids = ArrayHandler.Collected16
            });
        }
    }
    
    [HarmonyPatch(typeof(TransportTool))]
    [HarmonyPatch("StartEditingBuildingLine")]
    public class StartEditingBuildingLine
    {
        public static void Prefix()
        {
            if (IgnoreHelper.IsIgnored())
                return;

            ArrayHandler.StartCollecting();
            IgnoreHelper.StartIgnore("StartEditingBuildingLine");
        }
        
        public static void Postfix(TransportInfo info, ushort buildingID)
        {
            if (IgnoreHelper.IsIgnored("StartEditingBuildingLine"))
                return;

            IgnoreHelper.EndIgnore("StartEditingBuildingLine");
            ArrayHandler.StopCollecting();

            Command.SendToAll(new TransportLineStartEditBuildingCommand()
            {
                Array16Ids = ArrayHandler.Collected16,
                Prefab = (ushort) Mathf.Clamp(info.m_prefabDataIndex, 0, 65535),
                Building = buildingID
            });
        }
    }

    [HarmonyPatch(typeof(TransportTool))]
    [HarmonyPatch("SimulationStep")]
    public class SimulationStep
    {
        public static void Prefix(Vector3 ___m_hitPosition, bool ___m_fixedPlatform, int ___m_hoverStopIndex, int ___m_hoverSegmentIndex, int ___m_mode, ToolBase.ToolErrors ___m_errors, ushort ___m_tempLine, out DataStore __state)
        {
            __state = new DataStore();

            if (IgnoreHelper.IsIgnored())
                return;

            TransportHandler.TrackSimulationStep = true;
            TransportHandler.TrackTempLine = ___m_tempLine;
            TransportHandler.DidUpdatePaths = false;
            TransportHandler.DidUpdateLinesNow = false;

            // Save current values for comparison
            __state.hitPos = ___m_hitPosition;
            __state.fixedP = ___m_fixedPlatform;
            __state.hoverStop = ___m_hoverStopIndex;
            __state.hoverSegment = ___m_hoverSegmentIndex;
            __state.mode = ___m_mode;
            __state.errors = ___m_errors;
        }
        
        public static void Postfix(TransportInfo ___m_prefab, Vector3 ___m_hitPosition, bool ___m_fixedPlatform, int ___m_hoverStopIndex, int ___m_hoverSegmentIndex, int ___m_mode, ToolBase.ToolErrors ___m_errors, ref DataStore __state)
        {
            if (IgnoreHelper.IsIgnored())
                return;
            
            TransportHandler.TrackSimulationStep = false;

            // Only send when values have changed
            if (TransportHandler.DidUpdatePaths ||
                __state.hitPos != ___m_hitPosition || __state.fixedP != ___m_fixedPlatform ||
                __state.hoverStop != ___m_hoverStopIndex || __state.hoverSegment != ___m_hoverSegmentIndex ||
                __state.mode != ___m_mode || __state.errors != ___m_errors)
            {
                Command.SendToAll(new TransportLineSyncCommand()
                {
                    HitPosition = ___m_hitPosition,
                    FixedPlatform = ___m_fixedPlatform,
                    HoverStopIndex = ___m_hoverStopIndex,
                    HoverSegmentIndex = ___m_hoverSegmentIndex,
                    Mode = ___m_mode,
                    Errors = ___m_errors,
                    UpdateLines = TransportHandler.DidUpdateLinesNow,
                    UpdatePaths = TransportHandler.DidUpdatePaths
                });

                TransportHandler.DidUpdateLinesNow = false;
                TransportHandler.DidUpdatePaths = false;
            }
        }

        public class DataStore
        {
            public Vector3 hitPos;
            public bool fixedP;
            public int hoverStop;
            public int hoverSegment;
            public int mode;
            public ToolBase.ToolErrors errors;
        }
    }
    
    [HarmonyPatch(typeof(TransportTool))]
    [HarmonyPatch("EnsureTempLine")]
    public class EnsureTempLine
    {
        public static void Prefix(ushort ___m_tempLine, ushort ___m_lastEditLine, int ___m_lastMoveIndex, int ___m_lastAddIndex, Vector3 ___m_lastAddPos, out DataStore __state)
        {
            __state = new DataStore();
            
            if (IgnoreHelper.IsIgnored() || !TransportHandler.TrackSimulationStep)
                return;

            __state.tempLine = ___m_tempLine;
            __state.editLine = ___m_lastEditLine;
            __state.move = ___m_lastMoveIndex;
            __state.add = ___m_lastAddIndex;
            __state.addP = ___m_lastAddPos;

            ArrayHandler.StartCollecting();
            IgnoreHelper.StartIgnore("EnsureTempLine");
        }

        public static void Postfix(TransportInfo info, ushort sourceLine, int moveIndex, int addIndex, Vector3 addPos, bool fixedPlatform, ushort ___m_lastEditLine, ushort ___m_tempLine, ref DataStore __state)
        {
            if (IgnoreHelper.IsIgnored("EnsureTempLine") || !TransportHandler.TrackSimulationStep)
                return;

            IgnoreHelper.EndIgnore("EnsureTempLine");
            ArrayHandler.StopCollecting();

            // Make sure we send the command only when needed (Otherwise it would be really often!)
            if (ArrayHandler.Collected16.Length > 0 ||
                __state.tempLine != ___m_tempLine ||
                __state.editLine != ___m_lastEditLine ||
                __state.move != moveIndex ||
                __state.add != addIndex ||
                __state.addP != addPos)
            {
                if ((__state.addP == Vector3.zero || addPos == Vector3.zero) &&
                    moveIndex == __state.moveLast && addIndex == __state.addLast && addPos == __state.addPLast)
                {
                    return;
                }
                
                __state.addPLast = addPos;
                __state.moveLast = moveIndex;
                __state.addLast = addIndex;
                
                Command.SendToAll(new TransportLineTempCommand()
                {
                    InfoIndex = (ushort) Mathf.Clamp(info.m_prefabDataIndex, 0, 65535),
                    SourceLine = sourceLine,
                    MoveIndex = moveIndex,
                    AddIndex = addIndex,
                    AddPos = addPos,
                    FixedPlatform = fixedPlatform,
                    Array16Ids = ArrayHandler.Collected16
                });
            }
        }

        public class DataStore
        {
            public int moveLast, addLast;
            public Vector3 addPLast;
        
            public ushort tempLine, editLine;
            public int move, add;
            public Vector3 addP;
        }
    }

    [HarmonyPatch(typeof(TransportManager))]
    [HarmonyPatch("UpdateLinesNow")]
    public class UpdateLinesNow
    {
        public static void Postfix()
        {
            if (IgnoreHelper.IsIgnored() || !TransportHandler.TrackSimulationStep)
                return;

            TransportHandler.DidUpdateLinesNow = true;
        }
    }
    
    [HarmonyPatch(typeof(TransportLine))]
    [HarmonyPatch("UpdatePaths")]
    public class UpdatePaths
    {
        public static void Postfix(ushort lineID)
        {
            if (IgnoreHelper.IsIgnored() || !TransportHandler.TrackSimulationStep)
                return;

            if (TransportHandler.TrackTempLine == lineID)
            {
                TransportHandler.DidUpdatePaths = true;
            }
        }
    }

    [HarmonyPatch(typeof(TransportManager))]
    [HarmonyPatch("ReleaseLine")]
    public class ReleaseLine
    {
        public static void Prefix()
        {
            if (IgnoreHelper.IsIgnored())
                return;
            
            ArrayHandler.StartCollecting();
        }
        
        public static void Postfix(ushort lineID)
        {
            if (IgnoreHelper.IsIgnored())
                return;
            
            ArrayHandler.StopCollecting();
            
            Command.SendToAll(new TransportLineReleaseCommand()
            {
                Array16Ids = ArrayHandler.Collected16,
                Line = lineID
            });
        }
    }

    [HarmonyPatch(typeof(TransportLine))]
    public class MoveStopLine
    {
        public static void Prefix()
        {
            if (IgnoreHelper.IsIgnored())
                return;
            
            ArrayHandler.StartCollecting();
        }
        
        public static void Postfix(ushort lineID, int index, Vector3 newPos, bool fixedPlatform)
        {
            if (IgnoreHelper.IsIgnored())
                return;
            
            ArrayHandler.StopCollecting();
            
            Command.SendToAll(new TransportLineMoveStopManCommand()
            {
                Array16Ids = ArrayHandler.Collected16,
                Line = lineID,
                Index = index,
                NewPos = newPos,
                FixedPlatform = fixedPlatform
            });
        }
        
        public static MethodBase TargetMethod()
        {
            return typeof(TransportLine).GetMethod("MoveStop", AccessTools.all, null, new Type[] { typeof(ushort), typeof(int), typeof(Vector3), typeof(bool), typeof(Vector3).MakeByRefType() }, new ParameterModifier[] { });
        }
    }
}
