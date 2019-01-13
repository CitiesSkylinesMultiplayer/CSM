using ColossalFramework;
using CSM.Injections;
using CSM.Networking;
using System;
using UnityEngine;
using System.Reflection;
using static TerrainTool;
using Harmony;

namespace CSM.Commands.Handler
{
    class TerrainModificationHandler : CommandHandler<TerrainModificationCommand>
    {
        public override byte ID => CommandIds.TerrainModificationCommand;

        public override void HandleOnClient(TerrainModificationCommand command) => Handle(command);

        public override void HandleOnServer(TerrainModificationCommand command, Player player) => Handle(command);

        private void Handle(TerrainModificationCommand command)
        {
            TerrainHandler.ignoreTerrainModify = true;
            //lock (Singleton<TerrainTool>.instance)
            //{
                Singleton<TerrainTool>.instance.m_mode = command.mode;
                Singleton<TerrainTool>.instance.m_brushSize = command.BrushSize;
                Singleton<TerrainTool>.instance.m_strength = command.Strength;
                typeof(TerrainTool).GetField("m_startPosition", AccessTools.all).SetValue(ToolsModifierControl.GetTool<TerrainTool>(), command.StartPosition);
                typeof(TerrainTool).GetField("m_endPosition", AccessTools.all).SetValue(ToolsModifierControl.GetTool<TerrainTool>(), command.EndPosition);
                typeof(TerrainTool).GetField("m_mouseLeftDown", AccessTools.all).SetValue(ToolsModifierControl.GetTool<TerrainTool>(), command.MouseLeftDown);
                typeof(TerrainTool).GetField("m_mouseRightDown", AccessTools.all).SetValue(ToolsModifierControl.GetTool<TerrainTool>(), command.MouseRightDown);
                typeof(TerrainTool).GetField("m_mousePosition", AccessTools.all).SetValue(ToolsModifierControl.GetTool<TerrainTool>(), command.mousePosition);
                typeof(TerrainTool).GetField("m_strokeXmin", AccessTools.all).SetValue(ToolsModifierControl.GetTool<TerrainTool>(), command.StrokeXmin);
                typeof(TerrainTool).GetField("m_strokeXmax", AccessTools.all).SetValue(ToolsModifierControl.GetTool<TerrainTool>(), command.StrokeXmax);
                typeof(TerrainTool).GetField("m_strokeZmin", AccessTools.all).SetValue(ToolsModifierControl.GetTool<TerrainTool>(), command.StrokeZmin);
                typeof(TerrainTool).GetField("m_strokeZmax", AccessTools.all).SetValue(ToolsModifierControl.GetTool<TerrainTool>(), command.StrokeZmax);
                typeof(ToolController).GetField("m_BrushData", AccessTools.all).SetValue(Singleton<ToolController>.instance, command.BrushData); //this one is probably even more wrong but it has been testet without this one
                typeof(TerrainTool).GetMethod("ApplyBrush", AccessTools.all).Invoke(ToolsModifierControl.GetTool<TerrainTool>(), null);
            //}
            TerrainHandler.ignoreTerrainModify = false;
        }
    }

 }

