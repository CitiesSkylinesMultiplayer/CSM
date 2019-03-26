using ColossalFramework;
using CSM.Commands;
using CSM.Common;
using Harmony;
using UnityEngine;
using static ToolBase;

namespace CSM.Injections
{
    public class TerrainHandler
    {
        public static bool IgnoreAll = false;
    }

    [HarmonyPatch(typeof(TerrainTool))]
    [HarmonyPatch("ApplyBrush")]
    public class ApplyTerrainBrush
    {
        public static void Prefix()
        {
            TerrainTool tool = ToolsModifierControl.GetTool<TerrainTool>();
            if (!TerrainHandler.IgnoreAll && ReflectionHelper.GetAttr<ToolErrors>(tool, "m_toolErrors") == ToolErrors.None)
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
}
