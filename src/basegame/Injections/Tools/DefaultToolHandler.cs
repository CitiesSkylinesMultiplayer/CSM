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
    [HarmonyPatch(typeof(DefaultTool))]
    [HarmonyPatch("SimulationStep")]
    public class DefaultToolHandler {

        private static PlayerDefaultToolCommand _lastCommand;

        public static void Postfix(DefaultTool __instance, ToolController ___m_toolController, InstanceID ___m_hoverInstance, InstanceID ___m_hoverInstance2, int ___m_subHoverIndex, Vector3 ___m_mousePosition)
        {
            if (Command.CurrentRole != MultiplayerRole.None) {

                if (___m_toolController != null && ___m_toolController.IsInsideUI) {
                    return;
                }

                var newCommand = new PlayerDefaultToolCommand
                {
                    HoverInstanceID = ___m_hoverInstance,
                    HoverInstanceID2 = ___m_hoverInstance2,
                    SubIndex = ___m_subHoverIndex,
                    IsBulldozing = __instance is BulldozeTool,
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
    public class PlayerDefaultToolCommand : ToolCommandBase, IEquatable<PlayerDefaultToolCommand>
    {
        [ProtoMember(1)]
        public InstanceID HoverInstanceID { get; set; }

        [ProtoMember(2)]
        public InstanceID HoverInstanceID2 { get; set; }

        [ProtoMember(3)]
        public int SubIndex { get; set; }

        [ProtoMember(4)]
        public bool IsBulldozing { get; set; }

        // TODO: transmit placement errors
        public bool Equals(PlayerDefaultToolCommand other)
        {
            return base.Equals(other) &&
                   Equals(this.HoverInstanceID, other.HoverInstanceID) &&
                   Equals(this.HoverInstanceID2, other.HoverInstanceID2) &&
                   Equals(this.SubIndex, other.SubIndex) &&
                   Equals(this.IsBulldozing, other.IsBulldozing);
        }
    }

    public class PlayerDefaultToolCommandHandler : BaseToolCommandHandler<PlayerDefaultToolCommand, DefaultTool>
    {
        protected override void Configure(DefaultTool tool, ToolController toolController, PlayerDefaultToolCommand command) {
            // Note: Some private fields are already initialised by the ToolSimulator
            // These fields here are the important ones to transmit between game sessions
            ReflectionHelper.SetAttr(tool, "m_hoverInstance", command.HoverInstanceID);
            ReflectionHelper.SetAttr(tool, "m_hoverInstance2", command.HoverInstanceID2);
            ReflectionHelper.SetAttr(tool, "m_subHoverIndex", command.SubIndex);

            if (command.IsBulldozing)
            {
                tool.m_cursor = ToolsModifierControl.toolController.GetComponent<BulldozeTool>().m_cursor;
            }
            else {
                tool.m_cursor = ToolsModifierControl.toolController.GetComponent<DefaultTool>().m_cursor;
            }
        }

        protected override CursorInfo GetCursorInfo(DefaultTool tool)
        {
            return tool.m_cursor;
        }
    }
}
