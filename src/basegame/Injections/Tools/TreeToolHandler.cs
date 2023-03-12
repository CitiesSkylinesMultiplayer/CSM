using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Math;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using HarmonyLib;
using ProtoBuf;
using UnityEngine;

namespace CSM.BaseGame.Injections.Tools
{
    [HarmonyPatch(typeof(TreeTool))]
    [HarmonyPatch("OnToolUpdate")]
    public class TreeToolHandler {

        private static PlayerTreeToolCommand _lastCommand;

        public static void Postfix(TreeTool __instance, ToolController ___m_toolController, TreeInfo ___m_prefab, Vector3 ___m_cachedPosition, Randomizer ___m_randomizer, bool ___m_upgrading, ushort ___m_upgradeSegment, HashSet<ushort> ___m_upgradedSegments)
        {
            if (Command.CurrentRole != MultiplayerRole.None) {

                if (___m_toolController != null && ___m_toolController.IsInsideUI) {
                    return;
                }

                // Send info to all clients
                PlayerTreeToolCommand newCommand = new PlayerTreeToolCommand
                {                    
                    Tree = (uint)  ___m_prefab.m_prefabDataIndex,
                    Mode = (int) __instance.m_mode,
                    Position = ___m_cachedPosition,
                    RandomizerSeed = ___m_randomizer.seed,
                    Upgrading = ___m_upgrading,
                    UpgradeSegment = ___m_upgradeSegment,
                    UpgradedSegments = ___m_upgradedSegments.ToArray(),
                    BrushSize = __instance.m_brushSize,
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
    public class PlayerTreeToolCommand : ToolCommandBase, IEquatable<PlayerTreeToolCommand>
    {
        [ProtoMember(1)]
        public uint Tree { get; set; }
        [ProtoMember(2)]
        public int Mode { get; set; }
        [ProtoMember(3)]
        public Vector3 Position { get; set; }
        [ProtoMember(4)]
        public ulong RandomizerSeed { get; set; }
        [ProtoMember(5)]
        public bool Upgrading { get; set; }
        [ProtoMember(6)]
        public ushort UpgradeSegment { get; set; }
        [ProtoMember(7)]
        public ushort[] UpgradedSegments { get; set; }
        [ProtoMember(8)]
        public float BrushSize { get; set; }
        // TODO: Transmit placement errors

        public bool Equals(PlayerTreeToolCommand other)
        {
            return base.Equals(other) &&
                   Equals(this.Tree, other.Tree) &&
                   Equals(this.Mode, other.Mode) &&
                   Equals(this.Position, other.Position) &&
                   Equals(this.RandomizerSeed, other.RandomizerSeed) &&
                   Equals(this.Upgrading, other.Upgrading) &&
                   Equals(this.UpgradeSegment, other.UpgradeSegment) &&
                   this.UpgradedSegments.SequenceEqual(other.UpgradedSegments) &&
                   Equals(this.BrushSize, other.BrushSize);
        }
    }

    public class PlayerTreeToolCommandHandler : BaseToolCommandHandler<PlayerTreeToolCommand, TreeTool>
    {
        protected override void Configure(TreeTool tool, ToolController toolController, PlayerTreeToolCommand command) {
            // Note: Some private fields are already initialised by the ToolSimulator
            // These fields here are the important ones to transmit between game sessions

            ushort[] segments = command.UpgradedSegments ?? new ushort[0];
            
            ReflectionHelper.SetAttr(tool, "m_treeInfo", PrefabCollection<TreeInfo>.GetPrefab(command.Tree));
            tool.m_mode = (TreeTool.Mode) command.Mode;
            ReflectionHelper.SetAttr(tool, "m_cachedPosition", command.Position);
            ReflectionHelper.SetAttr(tool, "m_randomizer", new Randomizer(command.RandomizerSeed));
            ReflectionHelper.SetAttr(tool, "m_upgrading", command.Upgrading);
            ReflectionHelper.SetAttr(tool, "m_upgradeSegment", command.UpgradeSegment);
            ReflectionHelper.SetAttr(tool, "m_upgradedSegments", new HashSet<ushort>(segments));

            if (tool.m_mode == TreeTool.Mode.Brush)
            {
                toolController.SetBrush(tool.m_brush, command.Position, command.BrushSize);
            }
            else
            {
                toolController.SetBrush(null, Vector3.zero, 1f);
            }
        }

        protected override CursorInfo GetCursorInfo(TreeTool tool)
        {
            bool isUpgrading = ReflectionHelper.GetAttr<bool>(tool, "m_upgrading");            
            return isUpgrading ? tool.m_upgradeCursor : tool.m_buildCursor;
        }
    }
}
