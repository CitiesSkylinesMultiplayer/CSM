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
            Singleton<TerrainTool>.instance.m_mode = command.mode;
            Singleton<TerrainTool>.instance.m_brushSize = command.BrushSize;
            Singleton<TerrainTool>.instance.m_strength = command.Strength;
            typeof(TerrainTool).GetField("m_startPosition", AccessTools.all).SetValue(Singleton<TerrainTool>.instance, command.mode);
            typeof(TerrainTool).GetField("m_endPosition", AccessTools.all).SetValue(Singleton<TerrainTool>.instance, command.EndPosition);
            typeof(TerrainTool).GetField("m_mouseLeftDown", AccessTools.all).SetValue(Singleton<TerrainTool>.instance, command.MouseLeftDown); //I belive these are the once causing problems
            typeof(TerrainTool).GetField("m_mouseRightDown", AccessTools.all).SetValue(Singleton<TerrainTool>.instance, command.MouseRightDown);
            typeof(TerrainTool).GetField("m_mousePosition", AccessTools.all).SetValue(Singleton<TerrainTool>.instance, command.mousePosition);
            typeof(ToolController).GetField("m_BrushData", AccessTools.all).SetValue(Singleton<ToolBase>.instance, command.BrushData); //this one is probably even more wrong but it has been testet without this one
           
            typeof(TerrainTool).GetMethod("ApplyBrush", AccessTools.all).Invoke(ToolsModifierControl.GetTool<TerrainTool>(), null);
        }
    }

 }

