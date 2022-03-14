using ColossalFramework;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Terrain;
using HarmonyLib;
using UnityEngine;

namespace CSM.BaseGame.Injections
{
    [HarmonyPatch(typeof(TerrainTool))]
    [HarmonyPatch("ApplyBrush")]
    public class ApplyTerrainBrush
    {
        public static void Prefix()
        {
            TerrainTool tool = ToolsModifierControl.GetTool<TerrainTool>();
            if (!IgnoreHelper.Instance.IsIgnored() && ReflectionHelper.GetAttr<ToolBase.ToolErrors>(tool, "m_toolErrors") == ToolBase.ToolErrors.None)
            {
                Command.SendToAll(new TerrainModificationCommand
                {
                    BrushData = Singleton<ToolController>.instance.BrushData,
                    BrushSize = tool.m_brushSize,
                    StartPosition = ReflectionHelper.GetAttr<Vector3>(tool, "m_startPosition"),
                    EndPosition = ReflectionHelper.GetAttr<Vector3>(tool, "m_endPosition"),
                    MousePosition = ReflectionHelper.GetAttr<Vector3>(tool, "m_mousePosition"),
                    Mode = tool.m_mode,
                    Strength = tool.m_strength,
                    MouseRightDown = ReflectionHelper.GetAttr<bool>(tool, "m_mouseRightDown")
                });
            }
        }
    }

    [HarmonyPatch(typeof(TerrainManager))]
    [HarmonyPatch("DirtBuffer", MethodType.Setter)]
    public class SoilChanged
    {
        public static void Postfix(int ___m_dirtBuffer)
        {
            if (IgnoreHelper.Instance.IsIgnored())
            {
                return;
            }

            Command.SendToAll(new SoilTradeCommand
            {
                DirtBuffer = ___m_dirtBuffer
            });
        }
    }
}
