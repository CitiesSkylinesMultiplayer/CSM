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
    [HarmonyPatch(typeof(BuildingTool))]
    [HarmonyPatch("OnToolLateUpdate")]
    public class BuildingToolHandler {

        private static PlayerBuildingToolCommandHandler.Command lastCommand;
        // private static 

        public static void Postfix(BuildingTool __instance, Vector3 ___m_cachedPosition, float ___m_cachedAngle, int ___m_elevation, Segment3 ___m_cachedSegment)
        {
            if (MultiplayerManager.Instance.CurrentRole != MultiplayerRole.None) {

                // Send info to all clients
                var newCommand = new PlayerBuildingToolCommandHandler.Command
                {
                    Prefab = (ushort)Mathf.Clamp(__instance.m_prefab.m_prefabDataIndex, 0, 65535),
                    Relocating = __instance.m_relocate,
                    Position = ___m_cachedPosition,
                    Angle = ___m_cachedAngle,
                    Segment = ___m_cachedSegment,
                    Elevation = ___m_elevation
                };                
                if(!object.Equals(newCommand, lastCommand)) {
                    lastCommand = newCommand;
                    Command.SendToAll(newCommand);
                }

            }
        }    
    }

    public class PlayerBuildingToolCommandHandler : BaseToolCommandHandler<PlayerBuildingToolCommandHandler.Command, BuildingTool>
    {

        [ProtoContract]
        public class Command : CommandBase, IEquatable<Command>
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


            public bool Equals(Command other)
            {
                return object.Equals(this.Prefab, other.Prefab) &&
                object.Equals(this.Relocating, other.Relocating) &&
                object.Equals(this.Position, other.Position) &&
                object.Equals(this.Angle, other.Angle) &&
                object.Equals(this.Segment, other.Segment) &&
                object.Equals(this.Elevation, other.Elevation);
            }
            
        }

        protected override void Configure(BuildingTool tool, ToolController toolController, Command command) {
            BuildingInfo prefab = PrefabCollection<BuildingInfo>.GetPrefab(command.Prefab);
            ReflectionHelper.SetAttr(tool, "m_prefab", prefab);
            ReflectionHelper.SetAttr(tool, "m_relocate", command.Relocating);
            ReflectionHelper.SetAttr(tool, "m_cachedPosition", command.Position);
            ReflectionHelper.SetAttr(tool, "m_cachedAngle", command.Angle);
            ReflectionHelper.SetAttr(tool, "m_cachedSegment", command.Segment);
            ReflectionHelper.SetAttr(tool, "m_elevation", command.Elevation);
        }
    }

    
}