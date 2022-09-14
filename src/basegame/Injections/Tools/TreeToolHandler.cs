using System;
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

        private static PlayerTreeToolCommand lastCommand;

        public static void Postfix(TreeTool __instance, ToolController ___m_toolController, TreeInfo ___m_prefab, Vector3 ___m_cachedPosition, Randomizer ___m_randomizer, bool ___m_upgrading, ushort ___m_upgradeSegment, ushort[] ___m_upgradedSegments)
        {
            if (Command.CurrentRole != MultiplayerRole.None) {

                if (___m_toolController != null && ___m_toolController.IsInsideUI) {
                    return;
                }

                // Send info to all clients
                var newCommand = new PlayerTreeToolCommand
                {                    
                    Tree = (uint)  ___m_prefab.m_prefabDataIndex,
                    Mode = (int) __instance.m_mode,
                    Position = ___m_cachedPosition,
                    RandomizerSeed = ___m_randomizer.seed,
                    Upgrading = ___m_upgrading,
                    UpgradeSegment = ___m_upgradeSegment,
                    UpgradedSegments = ___m_upgradedSegments,
                    BrushSize = __instance.m_brushSize,
                    BrushData = ___m_toolController.BrushData,
                    CursorWorldPosition = ___m_cachedPosition,
                    PlayerName = Chat.Instance.GetCurrentUsername()
                };
                if (!newCommand.Equals(lastCommand)) {
                    lastCommand = newCommand;
                    Command.SendToAll(newCommand);
                }
                if(ToolSimulatorCursorManager.ShouldTest()) {
                    Command.GetCommandHandler(typeof(PlayerTreeToolCommand)).Parse(newCommand);
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
        [ProtoMember(9)]
        public float[] BrushData { get; set; }

        // TODO: Transmit brush info for clients to render. See TreeTool::OnToolUpdate
        // TODO: Transmit placement errors

        public bool Equals(PlayerTreeToolCommand other)
        {
            return base.Equals(other) &&
                   object.Equals(this.Tree, other.Tree) &&
                   object.Equals(this.Mode, other.Mode) &&
                   object.Equals(this.Position, other.Position) &&
                   object.Equals(this.RandomizerSeed, other.RandomizerSeed) &&
                   object.Equals(this.Upgrading, other.Upgrading) &&
                   object.Equals(this.UpgradeSegment, other.UpgradeSegment) &&
                   object.Equals(this.UpgradedSegments, other.UpgradedSegments) &&
                   object.Equals(this.BrushSize, other.BrushSize) &&
                   object.Equals(this.BrushData, other.BrushData);
        }
            
    }

    public class PlayerTreeToolCommandHandler : BaseToolCommandHandler<PlayerTreeToolCommand, TreeTool>
    {
        protected override void Configure(TreeTool tool, ToolController toolController, PlayerTreeToolCommand command) {
            // Note: Some private fields are already initialised by the ToolSimulator
            // These fields here are the important ones to transmit between game sessions
            
            ReflectionHelper.SetAttr(tool, "m_treeInfo", PrefabCollection<TreeInfo>.GetPrefab(command.Tree));
            tool.m_mode = (TreeTool.Mode) Enum.GetValues(typeof(TreeTool.Mode)).GetValue(command.Mode);
            ReflectionHelper.SetAttr(tool, "m_cachedPosition", command.Position);
            ReflectionHelper.SetAttr(tool, "m_randomizer", new Randomizer(command.RandomizerSeed));
            ReflectionHelper.SetAttr(tool, "m_upgrading", command.Upgrading);
            ReflectionHelper.SetAttr(tool, "m_upgradeSegment", command.UpgradeSegment);
            ReflectionHelper.SetAttr(tool, "m_upgradedSegments", command.UpgradedSegments);

            toolController.SetBrush(tool.m_brush, command.Position, command.BrushSize);
            ReflectionHelper.SetAttr(toolController, "m_brushData", command.BrushData);
            
            // TODO: send through m_upgradedSegments to show other segments that have been upgraded
            
        }

        protected override CursorInfo GetCursorInfo(TreeTool tool)
        {
            var isUpgrading = ReflectionHelper.GetAttr<bool>(tool, "m_upgrading");            
            return isUpgrading ? tool.m_upgradeCursor : tool.m_buildCursor;
        }
    }
}
