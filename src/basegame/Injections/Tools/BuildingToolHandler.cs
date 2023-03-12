using System;
using ColossalFramework;
using ColossalFramework.Math;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using HarmonyLib;
using JetBrains.Annotations;
using ProtoBuf;
using UnityEngine;

namespace CSM.BaseGame.Injections.Tools
{
    [HarmonyPatch(typeof(BuildingTool))]
    [HarmonyPatch("OnToolLateUpdate")]
    public class BuildingToolHandler {

        private static PlayerBuildingToolCommand _lastCommand;

        public static void Postfix(BuildingTool __instance, ToolController ___m_toolController, Vector3 ___m_cachedPosition, float ___m_cachedAngle, int ___m_elevation, Segment3 ___m_cachedSegment)
        {
            if (Command.CurrentRole != MultiplayerRole.None) {
                
                if (___m_toolController != null && ___m_toolController.IsInsideUI) {
                    return;
                }

                ushort prefabId;
                if(__instance.m_prefab != null) {
                    prefabId = (ushort)Mathf.Clamp(__instance.m_prefab.m_prefabDataIndex, 0, 65535);
                } else {
                    prefabId = 0;
                }

                // Send info to all clients
                PlayerBuildingToolCommand newCommand = new PlayerBuildingToolCommand
                {
                    Prefab = prefabId,
                    Relocating = __instance.m_relocate,
                    Position = ___m_cachedPosition,
                    Angle = ___m_cachedAngle,
                    Segment = ___m_cachedSegment,
                    Elevation = ___m_elevation,
                    CursorWorldPosition = ___m_cachedPosition,
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
    public class PlayerBuildingToolCommand : ToolCommandBase, IEquatable<PlayerBuildingToolCommand>
    {
        [ProtoMember(1)]
        public ushort Prefab { get; set; }
        [ProtoMember(2)]
        public int Relocating { get; set; }
        [ProtoMember(3)]
        public Vector3 Position { get; set; }
        [ProtoMember(4)]
        public float Angle { get; set; }
        [ProtoMember(5)]
        public Segment3 Segment { get; set; }
        [ProtoMember(6)]
        public int Elevation { get; set; }

        public bool Equals(PlayerBuildingToolCommand other)
        {
            return base.Equals(other) &&
                   Equals(this.Prefab, other.Prefab) &&
                   Equals(this.Relocating, other.Relocating) &&
                   Equals(this.Position, other.Position) &&
                   Equals(this.Angle, other.Angle) &&
                   Equals(this.Segment, other.Segment) &&
                   Equals(this.Elevation, other.Elevation);
        }
            
    }

    public class PlayerBuildingToolCommandHandler : BaseToolCommandHandler<PlayerBuildingToolCommand, BuildingTool>
    {
        protected override void Configure(BuildingTool tool, ToolController toolController, PlayerBuildingToolCommand command) {
            BuildingInfo prefab = PrefabCollection<BuildingInfo>.GetPrefab(command.Prefab);
            ReflectionHelper.SetAttr(tool, "m_prefab", prefab);
            ReflectionHelper.SetAttr(tool, "m_relocate", command.Relocating);
            ReflectionHelper.SetAttr(tool, "m_cachedPosition", command.Position);
            ReflectionHelper.SetAttr(tool, "m_cachedAngle", command.Angle);
            ReflectionHelper.SetAttr(tool, "m_cachedSegment", command.Segment);
            ReflectionHelper.SetAttr(tool, "m_elevation", command.Elevation);
        }

        protected override CursorInfo GetCursorInfo(BuildingTool tool)
        {
            return tool.m_buildCursor;
        }
    }
}
