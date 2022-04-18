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

        private static InstanceID lastInstanceId = InstanceID.Empty;
        private static InstanceID lastInstanceId2 = InstanceID.Empty;
        private static int lastSubIndex = -1;

        public static void Postfix(DefaultTool __instance)
        {

            if (MultiplayerManager.Instance.CurrentRole != MultiplayerRole.None) { 

                InstanceID hoverInstance;
                InstanceID hoverInstance2 = ReflectionHelper.GetAttr<InstanceID>(__instance, "m_hoverInstance2");
                int subIndex = -1;
                __instance.GetHoverInstance(out hoverInstance, out subIndex);
                
                if (hoverInstance != lastInstanceId || lastSubIndex != subIndex || lastInstanceId2 != hoverInstance2 ) {
                    lastInstanceId = hoverInstance;
                    lastSubIndex = subIndex;
                    lastInstanceId2 = hoverInstance2;

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
                    Command.SendToAll(new PlayerDefaultToolCommandHandler.Command
                    {
                        HoveredInstanceID = hoverInstance,
                        HoveredInstanceID2 = hoverInstance2,
                        SubIndex = subIndex
                    });
                }
            }
        }    
    }

    public class PlayerDefaultToolCommandHandler : CommandHandler<PlayerDefaultToolCommandHandler.Command>
    {

        [ProtoContract]
        public class Command : CommandBase
        {
            [ProtoMember(1)]
            public InstanceID HoveredInstanceID { get; set; }

            [ProtoMember(2)]
            public InstanceID HoveredInstanceID2 { get; set; }

            [ProtoMember(3)]
            public int SubIndex { get; set; }

            // TODO: transmit placement errors
            
        }

        protected override void Handle(Command command)
        {
            if (!MultiplayerManager.Instance.IsConnected())
            {
                // Ignore packets while not connected
                return;
            }

            var defaultTool = Singleton<ToolSimulator>.instance.GetTool<DefaultTool>(command.SenderId);
            ReflectionHelper.SetAttr(defaultTool, "m_hoveredInstance", command.HoveredInstanceID);
            ReflectionHelper.SetAttr(defaultTool, "m_hoveredInstance2", command.HoveredInstanceID2);
            ReflectionHelper.SetAttr(defaultTool, "m_subHoverIndex", command.SubIndex);
        }
    }

    
}