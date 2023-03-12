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
    [HarmonyPatch(typeof(ZoneTool))]
    [HarmonyPatch("SimulationStep")]
    public class ZoneToolHandler {

        private static PlayerZoneToolCommand _lastCommand;

        public static void Postfix(ZoneTool __instance, ToolController ___m_toolController, bool ___m_zoning, bool ___m_dezoning, bool ___m_validPosition, Vector3 ___m_startPosition, 
            Vector3 ___m_mousePosition, Vector3 ___m_startDirection, Vector3 ___m_mouseDirection, ulong[] ___m_fillBuffer2, Ray ___m_mouseRay, float ___m_mouseRayLength)
        {
            if (Command.CurrentRole != MultiplayerRole.None) {

                if (___m_toolController != null && ___m_toolController.IsInsideUI) {
                    return;
                }

                if (!___m_validPosition)
                {
                    ___m_mousePosition = Singleton<ToolSimulatorCursorManager>.instance.DoRaycast(___m_mouseRay, ___m_mouseRayLength);
                }

                // Send info to all clients
                PlayerZoneToolCommand newCommand = new PlayerZoneToolCommand
                {                    
                    Zone = (int) __instance.m_zone,
                    Mode = (int) __instance.m_mode,
                    BrushSize = __instance.m_brushSize,
                    Zoning = ___m_zoning,
                    Dezoning = ___m_dezoning,
                    ValidPosition = ___m_validPosition,
                    StartPosition = ___m_startPosition,
                    MousePosition = ___m_mousePosition,
                    StartDirection = ___m_startDirection,
                    MouseDirection = ___m_mouseDirection,
                    FillBuffer2 = ___m_fillBuffer2,
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
    public class PlayerZoneToolCommand : ToolCommandBase, IEquatable<PlayerZoneToolCommand>
    {
        [ProtoMember(1)]
        public int Zone { get; set; }
        [ProtoMember(2)]
        public int Mode { get; set; }
        [ProtoMember(3)]
        public float BrushSize { get; set; }
        [ProtoMember(4)]
        public bool Zoning { get; set; }
        [ProtoMember(5)]
        public bool Dezoning { get; set; }
        [ProtoMember(6)]
        public bool ValidPosition { get; set; }
        [ProtoMember(7)]
        public Vector3 StartPosition { get; set; }
        [ProtoMember(8)]
        public Vector3 MousePosition { get; set; }
        [ProtoMember(9)]
        public Vector3 StartDirection { get; set; }
        [ProtoMember(10)]
        public Vector3 MouseDirection { get; set; }
        [ProtoMember(11)]
        public ulong[] FillBuffer2 { get; set; }

        public bool Equals(PlayerZoneToolCommand other)
        {
            return base.Equals(other) &&
                   Equals(this.Zone, other.Zone) &&
                   Equals(this.Mode, other.Mode) &&
                   Equals(this.BrushSize, other.BrushSize) &&
                   Equals(this.Zoning, other.Zoning) &&
                   Equals(this.Dezoning, other.Dezoning) &&
                   Equals(this.ValidPosition, other.ValidPosition) &&
                   Equals(this.StartPosition, other.StartPosition) &&
                   Equals(this.MousePosition, other.MousePosition) &&
                   Equals(this.StartDirection, other.StartDirection) &&
                   Equals(this.MouseDirection, other.MouseDirection) &&
                   this.FillBuffer2.SequenceEqual(other.FillBuffer2);
        }
            
    }

    public class PlayerZoneToolCommandHandler : BaseToolCommandHandler<PlayerZoneToolCommand, ZoneTool>
    {
        protected override void Configure(ZoneTool tool, ToolController toolController, PlayerZoneToolCommand command) {
            // Note: Some private fields are already initialised by the ToolSimulator
            // These fields here are the important ones to transmit between game sessions
            tool.m_zone = (ItemClass.Zone) command.Zone;
            tool.m_mode = (ZoneTool.Mode) command.Mode;
            tool.m_brushSize = command.BrushSize;
            ReflectionHelper.SetAttr(tool, "m_zoning", command.Zoning);
            ReflectionHelper.SetAttr(tool, "m_dezoning", command.Dezoning);
            ReflectionHelper.SetAttr(tool, "m_validPosition", command.ValidPosition);
            ReflectionHelper.SetAttr(tool, "m_startPosition", command.StartPosition);
            ReflectionHelper.SetAttr(tool, "m_mousePosition", command.MousePosition);
            ReflectionHelper.SetAttr(tool, "m_startDirection", command.StartDirection);
            ReflectionHelper.SetAttr(tool, "m_mouseDirection", command.MouseDirection);
            ReflectionHelper.SetAttr(tool, "m_fillBuffer2", command.FillBuffer2);
        }

        protected override CursorInfo GetCursorInfo(ZoneTool tool)
        {
            if (tool.m_zoneCursors != null && (ItemClass.Zone)tool.m_zoneCursors.Length > tool.m_zone) {
                return tool.m_zoneCursors[(int)tool.m_zone];
            }
            return null;
        }
    }
}
