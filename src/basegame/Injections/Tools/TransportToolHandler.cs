using System;
using ColossalFramework;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using HarmonyLib;
using ProtoBuf;
using UnityEngine;

namespace CSM.BaseGame.Injections.Tools
{
    [HarmonyPatch(typeof(TransportTool))]
    [HarmonyPatch("SimulationStep")]
    public class TransportToolHandler {

        private static PlayerTransportToolCommand lastCommand;

        public static void Postfix(TransportTool __instance, ToolController ___m_toolController, ushort ___m_lastEditLine, int ___m_hoverStopIndex,
            int ___m_hoverSegmentIndex, Vector3 ___m_hitPosition, Ray ___m_mouseRay, float ___m_mouseRayLength)
        {
            if (Command.CurrentRole != MultiplayerRole.None) {

                if (___m_toolController != null && ___m_toolController.IsInsideUI) {
                    return;
                }
                Vector3 cursorPosition = Singleton<ToolSimulatorCursorManager>.instance.DoRaycast(___m_mouseRay, ___m_mouseRayLength);

                // Send info to all clients
                var newCommand = new PlayerTransportToolCommand
                {                    
                    TransportInfo = (uint)  __instance.m_prefab.m_prefabDataIndex,
                    LastEditLine = ___m_lastEditLine,
                    HoverStopIndex = ___m_hoverStopIndex,
                    HoverSegmentIndex = ___m_hoverSegmentIndex,
                    HitPosition = ___m_hitPosition,
                    CursorWorldPosition = cursorPosition,
                    PlayerName = Chat.Instance.GetCurrentUsername()
                };
                if (!newCommand.Equals(lastCommand)) {
                    lastCommand = newCommand;
                    Command.SendToAll(newCommand);
                }
                if(ToolSimulatorCursorManager.ShouldTest()) {
                    Command.GetCommandHandler(typeof(PlayerTransportToolCommand)).Parse(newCommand);
                }
            }
        }    
    }
    
    [ProtoContract]
    public class PlayerTransportToolCommand : ToolCommandBase, IEquatable<PlayerTransportToolCommand>
    {
        [ProtoMember(1)]
        public uint TransportInfo { get; set; }
        [ProtoMember(2)]
        public ushort LastEditLine { get; set; }
        [ProtoMember(3)]
        public int HoverStopIndex { get; set; }
        [ProtoMember(4)]
        public int HoverSegmentIndex { get; set; }
        [ProtoMember(5)]
        public Vector3 HitPosition { get; set; }

        // TODO: Transmit brush info for clients to render. See TransportTool::OnToolUpdate
        // TODO: Transmit placement errors

        public bool Equals(PlayerTransportToolCommand other)
        {
            return base.Equals(other) &&
                   object.Equals(this.TransportInfo, other.TransportInfo) &&
                   object.Equals(this.LastEditLine, other.LastEditLine) &&
                   object.Equals(this.HoverStopIndex, other.HoverStopIndex) &&
                   object.Equals(this.HoverSegmentIndex, other.HoverSegmentIndex) &&
                   object.Equals(this.HitPosition, other.HitPosition);
        }
            
    }

    public class PlayerTransportToolCommandHandler : BaseToolCommandHandler<PlayerTransportToolCommand, TransportTool>
    {
        protected override void Configure(TransportTool tool, ToolController toolController, PlayerTransportToolCommand command) {
            // TODO: somehow force the rendering to occur even when clients aren't viewing the transport layer
            tool.m_prefab = PrefabCollection<TransportInfo>.GetPrefab(command.TransportInfo);
            ReflectionHelper.SetAttr(tool, "m_lastEditLine", command.LastEditLine);
            ReflectionHelper.SetAttr(tool, "m_hoverStopIndex", command.HoverStopIndex);
            ReflectionHelper.SetAttr(tool, "m_hoverSegmentIndex", command.HoverSegmentIndex);
            ReflectionHelper.SetAttr(tool, "m_hitPosition", command.HitPosition);
        }

        protected override CursorInfo GetCursorInfo(TransportTool tool)
        {
            return null;
        }
    }
}
