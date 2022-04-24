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
    [HarmonyPatch(typeof(PropTool))]
    [HarmonyPatch("OnToolLateUpdate")]
    public class PropToolHandler {

        private static PlayerPropToolCommandHandler.Command lastCommand;

        public static void Postfix(PropTool __instance, PropInfo ___m_propInfo, Vector3 ___m_cachedPosition, float ___m_cachedAngle, Randomizer ___m_randomizer)
        {
            if (MultiplayerManager.Instance.CurrentRole != MultiplayerRole.None) {

                // Send info to all clients
                var newCommand = new PlayerPropToolCommandHandler.Command
                {                    
                    Prop = (uint)  ___m_propInfo.m_prefabDataIndex,
                    Mode = (int) __instance.m_mode,
                    Position = ___m_cachedPosition,
                    Angle = ___m_cachedAngle,
                    RandomizerSeed = ___m_randomizer.seed,
                    CursorWorldPosition = ___m_cachedPosition,
                    PlayerName = MultiplayerManager.Instance.CurrentUsername()
                };
                if(!object.Equals(newCommand, lastCommand)) {
                    lastCommand = newCommand;
                    Command.SendToAll(newCommand);
                }
                if(ToolSimulatorCursorManager.ShouldTest()) {
                    Command.GetCommandHandler(typeof(PlayerPropToolCommandHandler.Command)).Parse(newCommand);
                }

            }
        }    
    }

    public class PlayerPropToolCommandHandler : BaseToolCommandHandler<PlayerPropToolCommandHandler.Command, PropTool>
    {

        [ProtoContract]
        public class Command : ToolCommandBase, IEquatable<Command>
        {
            [ProtoMember(1)]
            public uint Prop { get; set; }
            [ProtoMember(2)]
            public int Mode { get; set; }
            [ProtoMember(3)]
            public Vector3 Position { get; set; }
            [ProtoMember(4)]
            public float Angle { get; set; }
            [ProtoMember(5)]
            public ulong RandomizerSeed { get; set; }

            // TODO: Transmit brush info for clients to render. See PropTool::OnToolUpdate
            // TODO: Transmit placement errors

            public bool Equals(Command other)
            {
                return base.Equals(other) &&
                object.Equals(this.Prop, other.Prop) &&
                object.Equals(this.Mode, other.Mode) &&
                object.Equals(this.Position, other.Position) &&
                object.Equals(this.Angle, other.Angle) &&
                object.Equals(this.RandomizerSeed, other.RandomizerSeed);
            }
            
        }

        protected override void Configure(PropTool tool, ToolController toolController, Command command) {
            // Note: Some private fields are already initialised by the ToolSimulator
            // These fields here are the important ones to transmit between game sessions
            
            ReflectionHelper.SetAttr(tool, "m_propInfo", PrefabCollection<PropInfo>.GetPrefab(command.Prop));
            tool.m_mode = (PropTool.Mode) Enum.GetValues(typeof(PropTool.Mode)).GetValue(command.Mode);
            ReflectionHelper.SetAttr(tool, "m_cachedPosition", command.Position);
            ReflectionHelper.SetAttr(tool, "m_cachedAngle", command.Angle);
            ReflectionHelper.SetAttr(tool, "m_randomizer", new Randomizer(command.RandomizerSeed));
            
        }

        protected override CursorInfo GetCursorInfo(PropTool tool)
        {
            return tool.m_buildCursor;
        }
    }

    
}