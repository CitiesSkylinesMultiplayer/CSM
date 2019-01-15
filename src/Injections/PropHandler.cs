using ColossalFramework;
using CSM.Commands;
using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CSM.Injections
{
    class PropHandler
    {
        public static List<ushort> IgnoreProp { get; } = new List<ushort>();
    }

    [HarmonyPatch(typeof(PropManager))]
    [HarmonyPatch("CreateProp")]
    public class CreateProp
    {
        public static void Postfix(bool __result, ref ushort prop, Vector3 position, float angle, bool single)
        {
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
            if (!PropHandler.IgnoreProp.Contains(prop))
            {
                Command.SendToAll(new PropMoveCommand
                {
                    PropID = prop,
                    Position = position
                });
            }
        }
    }

    [HarmonyPatch(typeof(PropManager))]
    [HarmonyPatch("ReleaseProp")]
    public class ReleaseProp
    {
        public static void Prefix(ushort prop)
        {
            if (!PropHandler.IgnoreProp.Contains(prop))
            {
                Command.SendToAll(new PropReleaseCommand
                {
                    PropID = prop
                });

            }
        }
    }
}

