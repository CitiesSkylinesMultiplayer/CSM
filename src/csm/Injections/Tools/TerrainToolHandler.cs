using System.Linq;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System;
using CSM.API.Commands;
using CSM.Networking;
using ICities;
using ColossalFramework;
using ColossalFramework.Math;
using ProtoBuf;
using HarmonyLib;
using UnityEngine;
using CSM.BaseGame.Helpers;
using CSM.API.Helpers;

namespace CSM.Injections.Tools
{
    [HarmonyPatch(typeof(TerrainTool))]
    [HarmonyPatch("SimulationStep")]
    public class TerrainToolHandler {

        private static PlayerTerrainToolCommandHandler.Command lastCommand;

        public static void Postfix(TerrainTool __instance, ToolController ___m_toolController, Vector3 ___m_mousePosition)
        {
            if (MultiplayerManager.Instance.CurrentRole != MultiplayerRole.None) {

                if (___m_toolController != null && ___m_toolController.IsInsideUI) {
                    return;
                }

                // Send info to all clients
                var newCommand = new PlayerTerrainToolCommandHandler.Command
                {
                    Mode = (uint)  __instance.m_mode,
                    MousePosition = ___m_mousePosition,
                    BrushSize = __instance.m_brushSize,
                    BrushData = ___m_toolController.BrushData,
                    CursorWorldPosition = ___m_mousePosition,
                    PlayerName = MultiplayerManager.Instance.CurrentUsername()
                };
                if(!object.Equals(newCommand, lastCommand)) {
                    lastCommand = newCommand;
                    Command.SendToAll(newCommand);
                }
                if(ToolSimulatorCursorManager.ShouldTest()) {
                    Command.GetCommandHandler(typeof(PlayerTerrainToolCommandHandler.Command)).Parse(newCommand);
                }

            }
        }    
    }

    public class PlayerTerrainToolCommandHandler : BaseToolCommandHandler<PlayerTerrainToolCommandHandler.Command, TerrainTool>
    {

        [ProtoContract]
        public class Command : ToolCommandBase, IEquatable<Command>
        {
            [ProtoMember(1)]
            public uint Mode { get; set; }
            [ProtoMember(2)]
            public Vector3 MousePosition { get; set; }
            [ProtoMember(3)]
            public float BrushSize { get; set; }
            [ProtoMember(4)]
            public float[] BrushData { get; set; }

            // TODO: Transmit brush texture for clients to render. See TerrainTool::OnToolUpdate
            // TODO: Transmit placement errors

            public bool Equals(Command other)
            {
                return base.Equals(other) &&
                object.Equals(this.Mode, other.Mode) &&
                object.Equals(this.MousePosition, other.MousePosition) &&
                object.Equals(this.BrushSize, other.BrushSize) &&
                object.Equals(this.BrushData, other.BrushData);
            }
            
        }

        protected override void Configure(TerrainTool tool, ToolController toolController, Command command) {            
            // The terrain tool uses to the tool controller to hold onto brush state and to render it
            tool.m_mode = (TerrainTool.Mode) Enum.GetValues(typeof(TerrainTool.Mode)).GetValue(command.Mode);
	        toolController.SetBrush(tool.m_brush, command.MousePosition, command.BrushSize);
            ReflectionHelper.SetAttr(toolController, "m_brushData", command.BrushData);
        }

        protected override CursorInfo GetCursorInfo(TerrainTool tool)
        {
            switch(tool.m_mode) {
                case TerrainTool.Mode.Shift: return tool.m_shiftCursor;
                case TerrainTool.Mode.Level: return tool.m_levelCursor;
                case TerrainTool.Mode.Slope: return tool.m_slopeCursor;
                case TerrainTool.Mode.Soften: return tool.m_softenCursor;
            }
            return null;
        }

        
    }

    
}