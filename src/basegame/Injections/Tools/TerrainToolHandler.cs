using System;
using System.Linq;
using ColossalFramework;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using HarmonyLib;
using ProtoBuf;
using UnityEngine;

namespace CSM.BaseGame.Injections.Tools
{
    [HarmonyPatch(typeof(TerrainTool))]
    [HarmonyPatch("SimulationStep")]
    public class TerrainToolHandler {

        private static PlayerTerrainToolCommand _lastCommand;

        public static void Postfix(TerrainTool __instance, ToolController ___m_toolController, Vector3 ___m_mousePosition)
        {
            if (Command.CurrentRole != MultiplayerRole.None) {

                if (___m_toolController != null && ___m_toolController.IsInsideUI) {
                    return;
                }

                // Send info to all clients
                PlayerTerrainToolCommand newCommand = new PlayerTerrainToolCommand
                {
                    Mode = (uint)  __instance.m_mode,
                    MousePosition = ___m_mousePosition,
                    BrushSize = __instance.m_brushSize,
                    CursorWorldPosition = ___m_mousePosition,
                    PlayerName = Chat.Instance.GetCurrentUsername()
                };
                if (!newCommand.Equals(_lastCommand)) {
                    _lastCommand = newCommand;
                    Command.SendToAll(newCommand);
                }
            }
        }    
    }
    
    [ProtoContract]
    public class PlayerTerrainToolCommand : ToolCommandBase, IEquatable<PlayerTerrainToolCommand>
    {
        [ProtoMember(1)]
        public uint Mode { get; set; }
        [ProtoMember(2)]
        public Vector3 MousePosition { get; set; }
        [ProtoMember(3)]
        public float BrushSize { get; set; }

        // TODO: Transmit placement errors

        public bool Equals(PlayerTerrainToolCommand other)
        {
            return base.Equals(other) &&
                   Equals(this.Mode, other.Mode) &&
                   Equals(this.MousePosition, other.MousePosition) &&
                   Equals(this.BrushSize, other.BrushSize);
        }
            
    }

    public class PlayerTerrainToolCommandHandler : BaseToolCommandHandler<PlayerTerrainToolCommand, TerrainTool>
    {
        protected override void Configure(TerrainTool tool, ToolController toolController, PlayerTerrainToolCommand command) {            
            // The terrain tool uses to the tool controller to hold onto brush state and to render it
            tool.m_mode = (TerrainTool.Mode) command.Mode;
	        toolController.SetBrush(tool.m_brush, command.MousePosition, command.BrushSize);
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
