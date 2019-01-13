using ColossalFramework;
using CSM.Commands;
using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static TerrainTool;

namespace CSM.Injections
{
    class TerrainHandler
    {
        public static bool ignoreTerrainModify = false; 
    }
[HarmonyPatch(typeof(TerrainTool))]
[HarmonyPatch("ApplyBrush")]
public class ApplyTerrainBrush
    {
        public static void Postfix()//ref float ___m_strength, ref float ___m_brushSize, ref Vector3 ___m_startPosition, ref Vector3 ___m_endPosition, ref bool ___m_mouseLeftDown, ref bool ___m_mouseRightDown, ref Vector3 m_mousePosition, ref Mode ___m_mode, ref int ___m_strokeXmin, ref int ___m_strokeXmax, ref int ___m_strokeZmin, ref int ___m_strokeZmax)
        {
            if (!TerrainHandler.ignoreTerrainModify)
            {
                UnityEngine.Debug.Log($"Applyed Brush, strength");
                
                Command.SendToAll(new TerrainModificationCommand
                {
                    BrushData = Singleton<ToolController>.instance.BrushData,//(float[])typeof(ToolController).GetField("BrushData", AccessTools.all).GetValue(Singleton<ToolController>.instance), //ï'm preatty sure this one is wrong, but its not this one that is causing the problem
                    BrushSize = Singleton<TerrainTool>.instance.m_brushSize,
                    StartPosition = (Vector3)typeof(TerrainTool).GetField("m_startPosition", AccessTools.all).GetValue(ToolsModifierControl.GetTool<TerrainTool>()),
                    EndPosition = (Vector3)typeof(TerrainTool).GetField("m_endPosition", AccessTools.all).GetValue(ToolsModifierControl.GetTool<TerrainTool>()),
                    mode = Singleton<TerrainTool>.instance.m_mode,
                    Strength = Singleton<TerrainTool>.instance.m_strength,
                    MouseLeftDown = (bool)typeof(TerrainTool).GetField("m_mouseLeftDown", AccessTools.all).GetValue(ToolsModifierControl.GetTool<TerrainTool>()),
                    MouseRightDown = (bool)typeof(TerrainTool).GetField("m_mouseRightDown", AccessTools.all).GetValue(ToolsModifierControl.GetTool<TerrainTool>()),
                    mousePosition = (Vector3)typeof(TerrainTool).GetField("m_mousePosition", AccessTools.all).GetValue(ToolsModifierControl.GetTool<TerrainTool>()),
                    StrokeXmin = (int)typeof(TerrainTool).GetField("m_strokeXmin", AccessTools.all).GetValue(ToolsModifierControl.GetTool<TerrainTool>()),
                    StrokeXmax = (int)typeof(TerrainTool).GetField("m_strokeXmax", AccessTools.all).GetValue(ToolsModifierControl.GetTool<TerrainTool>()),
                    StrokeZmin = (int)typeof(TerrainTool).GetField("m_strokeZmin", AccessTools.all).GetValue(ToolsModifierControl.GetTool<TerrainTool>()),
                    StrokeZmax = (int)typeof(TerrainTool).GetField("m_strokeZmax", AccessTools.all).GetValue(ToolsModifierControl.GetTool<TerrainTool>()),
                });
                
                
            }
        }
    }



}

