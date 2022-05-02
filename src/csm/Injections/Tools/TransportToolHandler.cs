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
    [HarmonyPatch(typeof(TransportTool))]
    [HarmonyPatch("SimulationStep")]
    public class TransportToolHandler {

        private static PlayerTransportToolCommandHandler.Command lastCommand;

        public static void Postfix(TransportTool __instance, ToolController ___m_toolController, ushort ___m_lastEditLine, int ___m_hoverStopIndex, int ___m_hoverSegmentIndex, Vector3 ___m_hitPosition)
        {
            if (MultiplayerManager.Instance.CurrentRole != MultiplayerRole.None) {

                if (___m_toolController != null && ___m_toolController.IsInsideUI) {
                    return;
                }

                // Send info to all clients
                var newCommand = new PlayerTransportToolCommandHandler.Command
                {                    
                    TransportInfo = (uint)  __instance.m_prefab.m_prefabDataIndex,
                    LastEditLine = ___m_lastEditLine,
                    HoverStopIndex = ___m_hoverStopIndex,
                    HoverSegmentIndex = ___m_hoverSegmentIndex,
                    HitPosition = ___m_hitPosition,
                    CursorWorldPosition = ___m_hitPosition,
                    PlayerName = MultiplayerManager.Instance.CurrentUsername()
                };
                if(!object.Equals(newCommand, lastCommand)) {
                    lastCommand = newCommand;
                    Command.SendToAll(newCommand);
                }
                if(ToolSimulatorCursorManager.ShouldTest()) {
                    Command.GetCommandHandler(typeof(PlayerTransportToolCommandHandler.Command)).Parse(newCommand);
                }

            }
        }    
    }

    public class PlayerTransportToolCommandHandler : BaseToolCommandHandler<PlayerTransportToolCommandHandler.Command, TransportTool>
    {

        [ProtoContract]
        public class Command : ToolCommandBase, IEquatable<Command>
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

            public bool Equals(Command other)
            {
                return base.Equals(other) &&
                object.Equals(this.TransportInfo, other.TransportInfo) &&
                object.Equals(this.LastEditLine, other.LastEditLine) &&
                object.Equals(this.HoverStopIndex, other.HoverStopIndex) &&
                object.Equals(this.HoverSegmentIndex, other.HoverSegmentIndex) &&
                object.Equals(this.HitPosition, other.HitPosition);
            }
            
        }

        protected override void Configure(TransportTool tool, ToolController toolController, Command command) {
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