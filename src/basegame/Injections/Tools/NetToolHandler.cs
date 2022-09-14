using System;
using System.Collections.Generic;
using ColossalFramework;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using HarmonyLib;
using ProtoBuf;
using UnityEngine;

namespace CSM.BaseGame.Injections.Tools
{
    [HarmonyPatch(typeof(NetTool))]
    [HarmonyPatch("OnToolLateUpdate")]
    public class NetToolHandler {

        private static PlayerNetToolCommand lastCommand;
        // private static 

        public static void Postfix(NetTool __instance, ToolController ___m_toolController, NetTool.ControlPoint[] ___m_cachedControlPoints, int ___m_cachedControlPointCount, ushort[] ___m_upgradedSegments)
        {
            if (Command.CurrentRole != MultiplayerRole.None) {

                if (___m_toolController != null && ___m_toolController.IsInsideUI) {
                    return;
                }

                // with the NetTool the world position of the cursor will match the current last control point base on the following logic
                // See NetTool::OnToolUpdate
                Vector3 worldPosition;                
                if (__instance.m_mode == NetTool.Mode.Upgrade && ___m_cachedControlPointCount >= 2)
                {
                    worldPosition = ___m_cachedControlPoints[1].m_position;
                }
                else
                {
                    worldPosition = ___m_cachedControlPoints[___m_cachedControlPointCount].m_position;
                }

                // Send info to all clients
                PlayerNetToolCommand newCommand = new PlayerNetToolCommand
                {
                    Prefab = (ushort)Mathf.Clamp(__instance.m_prefab.m_prefabDataIndex, 0, 65535),
                    Mode = (int) __instance.m_mode,
                    ControlPoints = ___m_cachedControlPoints,
                    ControlPointCount = ___m_cachedControlPointCount,
                    UpgradedSegments = ___m_upgradedSegments,
                    CursorWorldPosition = worldPosition,
                    PlayerName = Chat.Instance.GetCurrentUsername()
                };
                if (!newCommand.Equals(lastCommand)) {
                    lastCommand = newCommand;
                    Command.SendToAll(newCommand);
                }
                if (ToolSimulatorCursorManager.ShouldTest()) {
                    Command.GetCommandHandler(typeof(PlayerNetToolCommand)).Parse(newCommand);
                }

            }
        }    
    }
    
    [ProtoContract]
    public class PlayerNetToolCommand : ToolCommandBase, IEquatable<PlayerNetToolCommand>
    {
        [ProtoMember(1)]
        public ushort Prefab { get; set; }
        [ProtoMember(2)]
        public int Mode { get; set; }
        [ProtoMember(3)]
        public NetTool.ControlPoint[] ControlPoints { get; set; } 
        [ProtoMember(4)]
        public int ControlPointCount { get; set; }
        [ProtoMember(5)]
        public ushort[] UpgradedSegments { get; set; }

        // TODO: Transmit errors

        public bool Equals(PlayerNetToolCommand other)
        {
            return base.Equals(other) &&
                   object.Equals(this.Prefab, other.Prefab) &&
                   object.Equals(this.Mode, other.Mode) &&
                   object.Equals(this.ControlPoints, other.ControlPoints) &&
                   object.Equals(this.ControlPointCount, other.ControlPointCount) &&
                   object.Equals(this.UpgradedSegments, other.UpgradedSegments);
        }
            
    }

    public class PlayerNetToolCommandHandler : BaseToolCommandHandler<PlayerNetToolCommand, NetTool>
    {
        protected override void Configure(NetTool tool, ToolController toolController, PlayerNetToolCommand command) {
            // Note: Some private fields are already initialised by the ToolSimulator
            // These fields here are the important ones to transmit between game sessions
            NetInfo prefab = PrefabCollection<NetInfo>.GetPrefab(command.Prefab);
            ReflectionHelper.SetAttr(tool, "m_prefab", prefab);
            NetTool.Mode mode = (NetTool.Mode) Enum.GetValues(typeof(NetTool.Mode)).GetValue(command.Mode);
            tool.m_mode = mode;
            ReflectionHelper.SetAttr(tool, "m_cachedControlPoints", command.ControlPoints);
            ReflectionHelper.SetAttr(tool, "m_cachedControlPointCount", command.ControlPointCount);

            ushort[] segments;
            if(command.UpgradedSegments != null) {
                segments = command.UpgradedSegments;
            } else {
                segments = new ushort[0];
            }

            ReflectionHelper.SetAttr(tool, "m_upgradedSegments", new HashSet<ushort>(segments));
        }

        protected override CursorInfo GetCursorInfo(NetTool tool)
        {
            if (tool.m_mode == NetTool.Mode.Upgrade)
            {
                return tool.Prefab.m_upgradeCursor ?? tool.m_upgradeCursor;
            } else {
                return tool.Prefab.m_placementCursor ?? tool.m_placementCursor;
            }
        }
    }
}
