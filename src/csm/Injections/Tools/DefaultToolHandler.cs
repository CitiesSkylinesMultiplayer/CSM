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
    [HarmonyPatch(typeof(DefaultTool))]
    [HarmonyPatch("SimulationStep")]
    public class DefaultToolHandler {

        private static PlayerDefaultToolCommandHandler.Command lastCommand;

        public static void Postfix(DefaultTool __instance, InstanceID ___m_hoverInstance, InstanceID ___m_hoverInstance2, int ___m_subHoverIndex)
        {

            if (MultiplayerManager.Instance.CurrentRole != MultiplayerRole.None) {

                var newCommand = new PlayerDefaultToolCommandHandler.Command
                    {
                        HoverInstanceID = ___m_hoverInstance,
                        HoverInstanceID2 = ___m_hoverInstance2,
                        SubIndex = ___m_subHoverIndex
                    };
                if(!object.Equals(newCommand, lastCommand)) {
                    lastCommand = newCommand;
                    Command.SendToAll(newCommand);
                }
            }
        }    
    }

    public class PlayerDefaultToolCommandHandler : BaseToolCommandHandler<PlayerDefaultToolCommandHandler.Command, DefaultTool>
    {

        [ProtoContract]
        public class Command : CommandBase, IEquatable<Command>
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
                return object.Equals(this.HoverInstanceID, other.HoverInstanceID) &&
                object.Equals(this.HoverInstanceID2, other.HoverInstanceID2) &&
                object.Equals(this.SubIndex, other.SubIndex);
            }
            
        }

        protected override void Configure(DefaultTool tool, Command command) {
            // Note: Some private fields are already initialised by the ToolSimulator
            // These fields here are the important ones to transmit between game sessions
            ReflectionHelper.SetAttr(tool, "m_hoverInstance", command.HoverInstanceID);
            ReflectionHelper.SetAttr(tool, "m_hoverInstance2", command.HoverInstanceID2);
            ReflectionHelper.SetAttr(tool, "m_subHoverIndex", command.SubIndex);
        }
    }

    
}