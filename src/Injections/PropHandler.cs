using ColossalFramework;
using CSM.Commands;
using CSM.Commands.Data.Props;
using CSM.Helpers;
using HarmonyLib;
using UnityEngine;

namespace CSM.Injections
{
    [HarmonyPatch(typeof(PropManager))]
    [HarmonyPatch("CreateProp")]
    public class CreateProp
    {
        public static void Postfix(bool __result, ref ushort prop, Vector3 position, float angle, bool single)
        {
            if (IgnoreHelper.IsIgnored())
                return;

            if (__result)
            {
                PropInstance propInstance = Singleton<PropManager>.instance.m_props.m_buffer[prop];

                Command.SendToAll(new PropCreateCommand
                {
                    Position = position,
                    PropId = prop,
                    Single = single,
                    Angle = angle,
                    InfoIndex = propInstance.m_infoIndex
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
            if (IgnoreHelper.IsIgnored())
                return;

            Command.SendToAll(new PropMoveCommand
            {
                PropId = prop,
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
            if (IgnoreHelper.IsIgnored())
                return;

            Command.SendToAll(new PropReleaseCommand
            {
                PropId = prop
            });
        }
    }
}
