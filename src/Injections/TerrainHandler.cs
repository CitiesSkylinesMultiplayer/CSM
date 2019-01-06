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
    }
[HarmonyPatch(typeof(TerrainTool))]
[HarmonyPatch("ApplyBrush")]
public class ApplyTerrainBrush
    {
        public static void Postfix()
        {
            UnityEngine.Debug.Log("Applyed Brush");
            Command.SendToAll(new TerrainModificationCommand
            {
                BrushData = (float[])typeof(ToolController).GetField("m_BrushData", AccessTools.all).GetValue(Singleton<ToolBase>.instance), //ï'm preatty sure this one is wrong, but its not this one that is causing the problem
                BrushSize = Singleton<TerrainTool>.instance.m_brushSize,
                StartPosition = (Vector3)typeof(TerrainTool).GetField("m_startPosition", AccessTools.all).GetValue(Singleton<TerrainTool>.instance),
                EndPosition = (Vector3)typeof(TerrainTool).GetField("m_endPosition", AccessTools.all).GetValue(Singleton<TerrainTool>.instance),
                mode = Singleton<TerrainTool>.instance.m_mode,
                Strength = Singleton<TerrainTool>.instance.m_strength,
                MouseLeftDown = (bool)typeof(TerrainTool).GetField("m_mouseLeftDown", AccessTools.all).GetValue(Singleton<TerrainTool>.instance),
                MouseRightDown = (bool)typeof(TerrainTool).GetField("m_mouseRightDown", AccessTools.all).GetValue(Singleton<TerrainTool>.instance),
                mousePosition = (Vector3)typeof(TerrainTool).GetField("m_mousePosition", AccessTools.all).GetValue(Singleton<TerrainTool>.instance),              
        });
        }
    }



}

