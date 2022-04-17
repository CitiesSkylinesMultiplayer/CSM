using System.Xml.Serialization;
using System.Collections.ObjectModel;
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

                // Set the correct playerName if our currentRole is SERVER, else use the CurrentClient Username
                string playerName;
                if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
                {
                    playerName = MultiplayerManager.Instance.CurrentServer.Config.Username;
                }
                else
                {
                    playerName = MultiplayerManager.Instance.CurrentClient.Config.Username;
                }

                // Send info to all clients
                var newCommand = new PlayerBuildingToolCommandHandler.Command
                {
                    PlayerName = playerName,
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

    public class PlayerBuildingToolCommandHandler : CommandHandler<PlayerBuildingToolCommandHandler.Command>
    {

        [ProtoContract]
        public class Command : CommandBase, IEquatable<Command>
        {
            [ProtoMember(1)]
            public string PlayerName { get; set; }
            [ProtoMember(2)]
            public ushort Prefab { get; set; }
            [ProtoMember(3)]
            public int Relocating { get; set; }
            [ProtoMember(4)]
            public Vector3 Position { get; set; }
            [ProtoMember(5)]
            public float Angle { get; set; }
            [ProtoMember(6)]
            public Segment3 Segment { get; set; }
            [ProtoMember(7)]
            public int Elevation { get; set; }


            public bool Equals(Command other)
            {
                return object.Equals(this.PlayerName, other.PlayerName) &&
                object.Equals(this.Prefab, other.Prefab) &&
                object.Equals(this.Relocating, other.Relocating) &&
                object.Equals(this.Position, other.Position) &&
                object.Equals(this.Segment, other.Segment) &&
                object.Equals(this.Angle, other.Angle) &&
                object.Equals(this.Elevation, other.Elevation);
            }
            
        }

        public PlayerBuildingToolCommandHandler()
        {
            TransactionCmd = false;
        }

        protected override void Handle(Command command)
        {
            if (!MultiplayerManager.Instance.IsConnected())
            {
                // Ignore packets while not connected
                return;
            }
            var buildingTool = ToolSimulator.GetTool<BuildingTool>(command.SenderId);

            BuildingInfo prefab = PrefabCollection<BuildingInfo>.GetPrefab(command.Prefab);

            ReflectionHelper.SetAttr(buildingTool, "m_prefab", prefab);
            ReflectionHelper.SetAttr(buildingTool, "m_relocate", command.Relocating);
            ReflectionHelper.SetAttr(buildingTool, "m_cachedPosition", command.Position);
            ReflectionHelper.SetAttr(buildingTool, "m_cachedAngle", command.Angle);
            ReflectionHelper.SetAttr(buildingTool, "m_cachedSegment", command.Segment);
            ReflectionHelper.SetAttr(buildingTool, "m_elevation", command.Elevation);
            
        }
    }

    
}