using ColossalFramework;
using CSM.Commands;
using Harmony;
using UnityEngine;

namespace CSM.Injections
{
    public class PropHandler
    {
        public static bool IgnoreAll { get; set; } = false;
    }

    [HarmonyPatch(typeof(PropManager))]
    [HarmonyPatch("CreateProp")]
    public class CreateProp
    {
        public static void Postfix(bool __result, ref ushort prop, Vector3 position, float angle, bool single)
        {
            if (PropHandler.IgnoreAll)
                return;

            if (__result)
            {
                PropInstance propInstance = Singleton<PropManager>.instance.m_props.m_buffer[prop];

                Command.SendToAll(new PropCreateCommand
                {
                    position = position,
                    PropID = prop,
                    single = single,
                    angle = angle,
                    infoindex = propInstance.m_infoIndex 
                });
            }
        }
    }
    [HarmonyPatch(typeof(PropManager))]
    [HarmonyPatch("MoveProp")]
    public class MoveProp
    {
        public static void Postfix(ushort prop, Vector3 position)
        {
            if (PropHandler.IgnoreAll)
                return;

            Command.SendToAll(new PropMoveCommand
            {
                PropID = prop,
                Position = position
            });
        }
    }

    [HarmonyPatch(typeof(PropManager))]
    [HarmonyPatch("ReleaseProp")]
    public class ReleaseProp
    {
        public static void Prefix(ushort prop)
        {
            if (PropHandler.IgnoreAll)
                return;

            Command.SendToAll(new PropReleaseCommand
            {
                PropID = prop
            });
        }
    }
}
