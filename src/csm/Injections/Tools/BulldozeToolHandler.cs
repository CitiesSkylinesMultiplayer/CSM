using System;
using CSM.API;
using CSM.API.Commands;
using CSM.Networking;
using CSM.Panels;
using ProtoBuf;
using HarmonyLib;
using UnityEngine;
using ColossalFramework;
using CSM.Container;
using ColossalFramework.Math;
using CSM.BaseGame.Helpers;
using CSM.API.Helpers;

namespace CSM.Injections.Tools
{
    [HarmonyPatch(typeof(BulldozeTool))]
    [HarmonyPatch("SimulationStep")]
    public class BulldozeToolHandler {

        private static PlayerBulldozeToolCommandHandler.Command lastCommand;

        public static void Postfix(BulldozeTool __instance, ToolController ___m_toolController, InstanceID ___m_hoverInstance, InstanceID ___m_hoverInstance2, int ___m_subHoverIndex, Vector3 ___m_mousePosition)
        {
            
            if (MultiplayerManager.Instance.CurrentRole != MultiplayerRole.None) {

                if (___m_toolController != null && ___m_toolController.IsInsideUI) {
                    return;
                }

                var newCommand = new PlayerBulldozeToolCommandHandler.Command
                {
                    HoverInstanceID = ___m_hoverInstance,
                    HoverInstanceID2 = ___m_hoverInstance2,
                    SubIndex = ___m_subHoverIndex,
                    CursorWorldPosition = ___m_mousePosition,
                    PlayerName = MultiplayerManager.Instance.CurrentUsername()
                };
                if(!object.Equals(newCommand, lastCommand)) {
                    lastCommand = newCommand;
                    Command.SendToAll(newCommand);
                }

                if(ToolSimulatorCursorManager.ShouldTest()) {
                    Command.GetCommandHandler(typeof(PlayerBulldozeToolCommandHandler.Command)).Parse(newCommand);
                }
            }
        }    
    }

    public class PlayerBulldozeToolCommandHandler : BaseToolCommandHandler<PlayerBulldozeToolCommandHandler.Command, BulldozeTool>
    {

        [ProtoContract]
        public class Command : ToolCommandBase, IEquatable<Command>
        {
            [ProtoMember(1)]
            public InstanceID HoverInstanceID { get; set; }

            [ProtoMember(2)]
            public InstanceID HoverInstanceID2 { get; set; }

            [ProtoMember(3)]
            public int SubIndex { get; set; }

            // TODO: transmit placement errors
            public bool Equals(Command other)
            {
                return base.Equals(other) &&
                object.Equals(this.HoverInstanceID, other.HoverInstanceID) &&
                object.Equals(this.HoverInstanceID2, other.HoverInstanceID2) &&
                object.Equals(this.SubIndex, other.SubIndex);
            }
            
        }

        protected override void Configure(BulldozeTool tool, ToolController toolController, Command command) {
            // Note: Some private fields are already initialised by the ToolSimulator
            // These fields here are the important ones to transmit between game sessions
            ReflectionHelper.SetAttr(tool, "m_hoverInstance", command.HoverInstanceID);
            ReflectionHelper.SetAttr(tool, "m_hoverInstance2", command.HoverInstanceID2);
            ReflectionHelper.SetAttr(tool, "m_subHoverIndex", command.SubIndex);
        }

        protected override CursorInfo GetCursorInfo(BulldozeTool tool)
        {
            
            return tool.m_cursor;
        }
    }

    
}