using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.TransportLines;
using HarmonyLib;
using UnityEngine;

namespace CSM.BaseGame.Injections
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
            if (IgnoreHelper.Instance.IsIgnored())
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
            IgnoreHelper.Instance.StartIgnore("NewLine");
        }

        public static void Postfix(object __instance, ref bool __state)
        {
            if (IgnoreHelper.Instance.IsIgnored("NewLine"))
                return;

            if (!__state)
            {
                return;
            }

            IgnoreHelper.Instance.EndIgnore("NewLine");
            ArrayHandler.StopCollecting();

            TransportTool tool = ReflectionHelper.GetAttr<TransportTool>(__instance, "$this");

            ushort prefab = (ushort)Mathf.Clamp(tool.m_prefab.m_prefabDataIndex, 0, 65535);
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
            if (IgnoreHelper.Instance.IsIgnored())
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
            IgnoreHelper.Instance.StartIgnore("RemoveStop");
        }

        public static void Postfix(object __instance, ref bool __state)
        {
            if (IgnoreHelper.Instance.IsIgnored("RemoveStop"))
                return;

            if (!__state)
            {
                return;
            }

            TransportTool tool = ReflectionHelper.GetAttr<TransportTool>(__instance, "$this");

            IgnoreHelper.Instance.EndIgnore("RemoveStop");
            ArrayHandler.StopCollecting();

            int building = ReflectionHelper.GetAttr<int>(tool, "m_building");
            ushort prefab = (ushort)Mathf.Clamp(tool.m_prefab.m_prefabDataIndex, 0, 65535);

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
            if (IgnoreHelper.Instance.IsIgnored())
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
            IgnoreHelper.Instance.StartIgnore("AddStop");
        }

        public static void Postfix(ref bool __state)
        {
            if (IgnoreHelper.Instance.IsIgnored("AddStop"))
                return;

            if (!__state)
            {
                return;
            }

            IgnoreHelper.Instance.EndIgnore("AddStop");
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
            if (IgnoreHelper.Instance.IsIgnored())
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
            IgnoreHelper.Instance.StartIgnore("MoveStop");
        }

        public static void Postfix(ref bool __state, bool ___applyChanges)
        {
            if (IgnoreHelper.Instance.IsIgnored("MoveStop"))
                return;

            if (!__state)
            {
                return;
            }

            IgnoreHelper.Instance.EndIgnore("MoveStop");
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
            if (IgnoreHelper.Instance.IsIgnored())
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
            IgnoreHelper.Instance.StartIgnore("CancelPrevStop");
        }

        public static void Postfix(object __instance, ref bool __state)
        {
            if (IgnoreHelper.Instance.IsIgnored("CancelPrevStop"))
                return;

            if (!__state)
            {
                return;
            }

            IgnoreHelper.Instance.EndIgnore("CancelPrevStop");
            ArrayHandler.StopCollecting();

            TransportTool tool = ReflectionHelper.GetAttr<TransportTool>(__instance, "$this");
            ushort prefab = (ushort)Mathf.Clamp(tool.m_prefab.m_prefabDataIndex, 0, 65535);
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
            if (IgnoreHelper.Instance.IsIgnored())
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
            IgnoreHelper.Instance.StartIgnore("CancelMoveStop");
        }

        public static void Postfix(object __instance, ref bool __state)
        {
            if (IgnoreHelper.Instance.IsIgnored("CancelMoveStop"))
                return;

            if (!__state)
            {
                return;
            }

            IgnoreHelper.Instance.EndIgnore("CancelMoveStop");
            ArrayHandler.StopCollecting();

            TransportTool tool = ReflectionHelper.GetAttr<TransportTool>(__instance, "$this");
            ushort prefab = (ushort)Mathf.Clamp(tool.m_prefab.m_prefabDataIndex, 0, 65535);

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
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            Command.SendToAll(new TransportLineInitCommand());
        }
    }

    [HarmonyPatch(typeof(TransportTool))]
    [HarmonyPatch("ResetTool")]
    public class ResetTool
    {
        public static void Prefix(TransportTool __instance, ushort ___m_tempLine)
        {
            // If is simulated instance
            if (__instance != ToolsModifierControl.GetTool<TransportTool>())
            {
                _oldLine = ReflectionHelper.GetAttr<ushort>(__instance, "m_lastEditLine");
                if (_oldLine != 0)
                {
                    _oldFlags = Singleton<TransportManager>.instance.m_lines.m_buffer[_oldLine].m_flags;
                }

                TransportManager instance = Singleton<TransportManager>.instance;
                if (___m_tempLine != 0)
                {
                    // Simulate temp line cleanup (doesn't have temporary flag)
                    instance.m_lines.m_buffer[___m_tempLine].CloneLine(___m_tempLine, 0);
                    instance.m_lines.m_buffer[___m_tempLine].UpdateMeshData(___m_tempLine);
                    instance.m_lines.m_buffer[___m_tempLine].m_flags |= TransportLine.Flags.Hidden;
                }
            }
            if (IgnoreHelper.Instance.IsIgnored())
            {
                return;
            }

            ArrayHandler.StartCollecting();
            IgnoreHelper.Instance.StartIgnore("ResetTool");
        }

        public static void Postfix(TransportTool __instance)
        {
            if (__instance != ToolsModifierControl.GetTool<TransportTool>())
            {
                // This prevents simulated tools from modifying the line flags of the "edit line"
                // as this might interfere with the current player's transport tool.
                if (_oldLine != 0)
                {
                    Singleton<TransportManager>.instance.m_lines.m_buffer[_oldLine].m_flags = _oldFlags;
                }
            }
            if (IgnoreHelper.Instance.IsIgnored("ResetTool"))
            {
                return;
            }

            IgnoreHelper.Instance.EndIgnore("ResetTool");
            ArrayHandler.StopCollecting();

            Command.SendToAll(new TransportLineResetCommand()
            {
                Array16Ids = ArrayHandler.Collected16
            });
        }

        private static ushort _oldLine;
        private static TransportLine.Flags _oldFlags;
    }

    [HarmonyPatch(typeof(TransportTool))]
    [HarmonyPatch("StartEditingBuildingLine")]
    public class StartEditingBuildingLine
    {
        public static void Prefix()
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            ArrayHandler.StartCollecting();
            IgnoreHelper.Instance.StartIgnore("StartEditingBuildingLine");
        }

        public static void Postfix(TransportInfo info, ushort buildingID)
        {
            if (IgnoreHelper.Instance.IsIgnored("StartEditingBuildingLine"))
                return;

            IgnoreHelper.Instance.EndIgnore("StartEditingBuildingLine");
            ArrayHandler.StopCollecting();

            Command.SendToAll(new TransportLineStartEditBuildingCommand()
            {
                Array16Ids = ArrayHandler.Collected16,
                Prefab = (ushort)Mathf.Clamp(info.m_prefabDataIndex, 0, 65535),
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

            if (IgnoreHelper.Instance.IsIgnored())
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

        public static void Postfix(Vector3 ___m_hitPosition, bool ___m_fixedPlatform, int ___m_hoverStopIndex, int ___m_hoverSegmentIndex, int ___m_mode, ToolBase.ToolErrors ___m_errors, ref DataStore __state)
        {
            if (IgnoreHelper.Instance.IsIgnored())
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
        public static bool Prefix(TransportTool __instance, TransportInfo info, ushort sourceLine, int moveIndex, int addIndex, Vector3 addPos, bool fixedPlatform,
            ref ushort ___m_tempLine, ref int ___m_lastMoveIndex, ref int ___m_lastAddIndex, ref Vector3 ___m_lastAddPos, ref Vector3 ___m_lastMovePos, out bool __result)
        {
            // Don't run this method for the simulated tool
            if (__instance != ToolsModifierControl.GetTool<TransportTool>())
            {
                __result = true;
                return false;
            }

            bool trackChanges = !IgnoreHelper.Instance.IsIgnored() && TransportHandler.TrackSimulationStep;
            if (trackChanges)
            {
                ArrayHandler.StartCollecting();
                IgnoreHelper.Instance.StartIgnore("EnsureTempLine");
            }

            int lastAddIndex = ___m_lastAddIndex;
            int lastMoveIndex = ___m_lastMoveIndex;
            Vector3 lastAddPos = ___m_lastAddPos;
            Vector3 lastMovePos = ___m_lastMovePos;

            // Replace the original EnsureTempLine method. Needed to be able to detect if an update should be sent out
            __result = SimulateEnsureTempLine(__instance, info, sourceLine, moveIndex, addIndex, addPos, fixedPlatform,
                ref ___m_tempLine, ref ___m_lastAddIndex, ref ___m_lastMoveIndex, ref ___m_lastAddPos, ref ___m_lastMovePos,
                out bool updateNeeded, out bool forceSetEditLine, out bool createLine, out ushort[] releaseLines);

            if (trackChanges)
            {
                IgnoreHelper.Instance.EndIgnore("EnsureTempLine");
                ArrayHandler.StopCollecting();

                if (updateNeeded)
                {
                    Command.SendToAll(new TransportLineTempCommand()
                    {
                        InfoIndex = (ushort)Mathf.Clamp(info.m_prefabDataIndex, 0, 65535),
                        ForceSetEditLine = forceSetEditLine,
                        TempLine = ___m_tempLine,
                        ReleaseLines = releaseLines,
                        CreateLine = createLine,
                        SourceLine = sourceLine,
                        MoveIndex = moveIndex,
                        AddIndex = addIndex,
                        AddPos = addPos,
                        LastAddIndex = lastAddIndex,
                        LastMoveIndex = lastMoveIndex,
                        LastAddPos = lastAddPos,
                        LastMovePos = lastMovePos,
                        FixedPlatform = fixedPlatform,
                        Array16Ids = ArrayHandler.Collected16
                    });
                }
            }

            return false;
        }

        private static bool SimulateEnsureTempLine(TransportTool tool, TransportInfo info, ushort sourceLine, int moveIndex, int addIndex, Vector3 addPos, bool fixedPlatform,
            ref ushort m_tempLine, ref int m_lastAddIndex, ref int m_lastMoveIndex, ref Vector3 m_lastAddPos, ref Vector3 m_lastMovePos,
            out bool updateNeeded, out bool forceSetEditLine, out bool createLine, out ushort[] releaseLines)
        {
            updateNeeded = false;
            forceSetEditLine = false;
            createLine = false;
            List<ushort> releaseLinesList = new List<ushort>();

            TransportManager instance = Singleton<TransportManager>.instance;
			if (m_tempLine != 0)
			{
				if ((instance.m_lines.m_buffer[m_tempLine].m_flags & TransportLine.Flags.Temporary) == 0)
				{
					m_tempLine = 0;
					ReflectionHelper.Call(tool, "SetEditLine", 0, true);
                    updateNeeded = true;
                    forceSetEditLine = true;
                }
				else if ((object)instance.m_lines.m_buffer[m_tempLine].Info != info)
				{
					instance.ReleaseLine(m_tempLine);
                    releaseLinesList.Add(m_tempLine);
					m_tempLine = 0;
					ReflectionHelper.Call(tool, "SetEditLine", 0, true);
                    updateNeeded = true;
                    forceSetEditLine = true;
				}
			}
            if (m_tempLine == 0)
            {
                for (ushort i = 1; i < 256; i++)
                {
                    if ((instance.m_lines.m_buffer[i].m_flags & TransportLine.Flags.Temporary) != 0)
                    {
                        if ((object)instance.m_lines.m_buffer[i].Info != info)
                        {
                            instance.ReleaseLine(i);
                            releaseLinesList.Add(i);
                            updateNeeded = true;
                            break;
                        }
                        m_tempLine = i;
                        ReflectionHelper.Call(tool, "SetEditLine", sourceLine, true);
                        updateNeeded = true;
                        forceSetEditLine = true;
                        break;
                    }
                }
            }
			if (m_tempLine == 0 && Singleton<TransportManager>.instance.CreateLine(out m_tempLine, ref Singleton<SimulationManager>.instance.m_randomizer, info, newNumber: false))
			{
				instance.m_lines.m_buffer[m_tempLine].m_flags |= TransportLine.Flags.Temporary;
				ReflectionHelper.Call(tool, "SetEditLine", sourceLine, true);
                updateNeeded = true;
                forceSetEditLine = true;
                createLine = true;
            }

            releaseLines = releaseLinesList.ToArray();
			if (m_tempLine != 0)
			{
                // Check if SetEditLine will change something
                if (sourceLine != ReflectionHelper.GetAttr<ushort>(tool, "m_lastEditLine"))
                {
                    updateNeeded = true;
                }
				ReflectionHelper.Call(tool, "SetEditLine", sourceLine, false);
				if (m_lastMoveIndex != moveIndex
                    || m_lastAddIndex != addIndex
                    || m_lastAddPos != addPos)
				{
					if (m_lastAddIndex != -2 && instance.m_lines.m_buffer[m_tempLine].RemoveStop(m_tempLine, m_lastAddIndex))
					{
						m_lastAddIndex = -2;
						m_lastAddPos = Vector3.zero;
                        updateNeeded = true;
					}
					if (m_lastMoveIndex != -2 && instance.m_lines.m_buffer[m_tempLine].MoveStop(m_tempLine, m_lastMoveIndex, m_lastMovePos, fixedPlatform))
					{
						m_lastMoveIndex = -2;
						m_lastMovePos = Vector3.zero;
                        updateNeeded = true;
					}
					instance.m_lines.m_buffer[m_tempLine].CopyMissingPaths(sourceLine);
					if (moveIndex != -2 && instance.m_lines.m_buffer[m_tempLine].MoveStop(m_tempLine, moveIndex, addPos, fixedPlatform, out var oldPos))
					{
						m_lastMoveIndex = moveIndex;
						m_lastMovePos = oldPos;
						m_lastAddPos = addPos;
                        updateNeeded = true;
					}
					if (addIndex != -2 && instance.m_lines.m_buffer[m_tempLine].AddStop(m_tempLine, addIndex, addPos, fixedPlatform))
					{
						m_lastAddIndex = addIndex;
						m_lastAddPos = addPos;
                        updateNeeded = true;
					}
					instance.UpdateLine(m_tempLine);
				}

				instance.m_lines.m_buffer[m_tempLine].m_color = instance.m_lines.m_buffer[sourceLine].m_color;
                if ((instance.m_lines.m_buffer[m_tempLine].m_flags & TransportLine.Flags.Hidden) != 0)
                {
                    instance.m_lines.m_buffer[m_tempLine].m_flags &= ~TransportLine.Flags.Hidden;
                    updateNeeded = true;
                }

                if ((instance.m_lines.m_buffer[sourceLine].m_flags & TransportLine.Flags.CustomColor) != 0)
				{
                    if ((instance.m_lines.m_buffer[m_tempLine].m_flags & TransportLine.Flags.CustomColor) == 0)
                    {
                        instance.m_lines.m_buffer[m_tempLine].m_flags |= TransportLine.Flags.CustomColor;
                        updateNeeded = true;
                    }
				}
				else
				{
                    if ((instance.m_lines.m_buffer[m_tempLine].m_flags & TransportLine.Flags.CustomColor) != 0)
                    {
                        instance.m_lines.m_buffer[m_tempLine].m_flags &= ~TransportLine.Flags.CustomColor;
                        updateNeeded = true;
                    }
				}
				return true;
			}
			ReflectionHelper.Call(tool, "SetEditLine", 0, false);
            updateNeeded = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(TransportManager))]
    [HarmonyPatch("UpdateLinesNow")]
    public class UpdateLinesNow
    {
        public static void Postfix()
        {
            if (IgnoreHelper.Instance.IsIgnored() || !TransportHandler.TrackSimulationStep)
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
            if (IgnoreHelper.Instance.IsIgnored() || !TransportHandler.TrackSimulationStep)
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
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            ArrayHandler.StartCollecting();
        }

        public static void Postfix(ushort lineID)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            ArrayHandler.StopCollecting();

            Command.SendToAll(new TransportLineReleaseCommand()
            {
                Array16Ids = ArrayHandler.Collected16,
                Line = lineID
            });
        }
    }

    [HarmonyPatch]
    public class MoveStopLine
    {
        public static void Prefix()
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            ArrayHandler.StartCollecting();
        }

        public static void Postfix(ushort lineID, int index, Vector3 newPos, bool fixedPlatform)
        {
            if (IgnoreHelper.Instance.IsIgnored())
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

    [HarmonyPatch(typeof(PublicTransportWorldInfoPanel))]
    [HarmonyPatch("OnVehicleCountModifierChanged")]
    public class ChangeVehicleCount
    {
        public static void Prefix(float value, PublicTransportWorldInfoPanel __instance)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            ushort lineId = ReflectionHelper.Call<ushort>(__instance, "GetLineID");

            if ((ushort)value == TransportManager.instance.m_lines.m_buffer[lineId].m_budget)
                return;

            Command.SendToAll(new TransportLineChangeSliderCommand()
            {
                LineId = lineId,
                Value = value,
                IsTicketPrice = false
            });
        }
    }

    [HarmonyPatch(typeof(PublicTransportWorldInfoPanel))]
    [HarmonyPatch("OnTicketPriceChanged")]
    public class ChangeTicketPrice
    {
        public static void Prefix(float value, PublicTransportWorldInfoPanel __instance)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            ushort lineId = ReflectionHelper.Call<ushort>(__instance, "GetLineID");

            if ((ushort)value == TransportManager.instance.m_lines.m_buffer[lineId].m_ticketPrice)
                return;

            Command.SendToAll(new TransportLineChangeSliderCommand()
            {
                LineId = lineId,
                Value = value,
                IsTicketPrice = true
            });
        }
    }

    [HarmonyPatch(typeof(TransportManager))]
    [HarmonyPatch("SetLineColor")]
    public class SetLineColor
    {
        public static void Prefix(ushort lineID, Color color)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            Command.SendToAll(new TransportLineChangeColorCommand()
            {
                LineId = lineID,
                Color = color
            });
        }
    }

    [HarmonyPatch]
    public class SetActiveParent
    {
        public static void Prefix(ushort id)
        {
            SetActive.trackingLineId = id;
        }

        public static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (Type t in new Type[] { typeof(PublicTransportLineInfo), typeof(PublicTransportWorldInfoPanel) })
            {
                foreach (string method in new string[] { "SetDayOnly", "SetNightOnly", "SetAllDay" })
                {
                    yield return t.GetMethod(method, ReflectionHelper.AllAccessFlags, null, new Type[] { typeof(ushort) }, null);
                }
            }
        }
    }

    [HarmonyPatch(typeof(TransportLine))]
    [HarmonyPatch("SetActive")]
    public class SetActive
    {
        public static ushort trackingLineId = 0;

        public static void Prefix(bool day, bool night)
        {
            if (IgnoreHelper.Instance.IsIgnored() || trackingLineId == 0)
                return;

            Command.SendToAll(new TransportLineChangeActiveCommand()
            {
                LineId = trackingLineId,
                Day = day,
                Night = night
            });

            trackingLineId = 0;
        }
    }

    [HarmonyPatch(typeof(TransportLine))]
    [HarmonyPatch("ReplaceVehicles")]
    public class ReplaceVehicles
    {
        public static void Postfix(ushort lineID, VehicleInfo info)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            // TODO: When info is null, random vehicle gets generated on spawn
            // To be in sync, we need to sync the randomization!
            uint? vehicle = null;
            if (info != null)
            {
                vehicle = (uint) info.m_prefabDataIndex;
            }

            Command.SendToAll(new TransportLineChangeVehicleCommand()
            {
                LineId = lineID,
                Vehicle = vehicle
            });
        }
    }

    // This patch prevents simulated tools from modifying the line flags of the "edit line"
    // as this might interfere with the current player's transport tool.
    [HarmonyPatch(typeof(TransportTool))]
    [HarmonyPatch("SetEditLine")]
    public class SetEditLine
    {
        public static void Prefix(TransportTool __instance, ushort line)
        {
            if (__instance == ToolsModifierControl.GetTool<TransportTool>())
            {
                // If this is the instance of the current player, don't do anything
                return;
            }

            _oldLine = ReflectionHelper.GetAttr<ushort>(__instance, "m_lastEditLine");
            if (_oldLine != 0)
            {
                _oldFlags = Singleton<TransportManager>.instance.m_lines.m_buffer[_oldLine].m_flags;
            }
            if (line != 0)
            {
                _newFlags = Singleton<TransportManager>.instance.m_lines.m_buffer[line].m_flags;
            }
        }

        public static void Postfix(TransportTool __instance, ushort line)
        {
            if (__instance == ToolsModifierControl.GetTool<TransportTool>())
            {
                // If this is the instance of the current player, don't do anything
                return;
            }

            if (_oldLine != 0)
            {
                Singleton<TransportManager>.instance.m_lines.m_buffer[_oldLine].m_flags = _oldFlags;
            }
            if (line != 0)
            {
                Singleton<TransportManager>.instance.m_lines.m_buffer[line].m_flags = _newFlags;
            }
        }

        private static ushort _oldLine;
        private static TransportLine.Flags _oldFlags;
        private static TransportLine.Flags _newFlags;
    }
}
