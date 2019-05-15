using CSM.Commands;
using Harmony;
using NLog;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CSM.Injections
{
    public class DistrictHandler
    {
        public static bool IgnoreAll { get; set; } = false;
    }

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("CreateDistrict")]
    public class CreateDistrict
    {
        public static void Postfix(bool __result, ref byte district)
        {
            if (DistrictHandler.IgnoreAll)
                return;

            if (__result)
            {
                Command.SendToAll(new DistrictCreateCommand
                {
                    DistrictID = district
                });
            }
        }
    }

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("SetDistrictName")]
    public class SetDistrictName
    {
        public static void Postfix(IEnumerator<bool> __result, byte district, string name)
        {
            if (DistrictHandler.IgnoreAll)
                return;

            Command.SendToAll(new DistrictNameCommand
            {
                DistrictID = district,
                Name = name
            });
        }
    }

    [HarmonyPatch(typeof(DistrictTool))]
    [HarmonyPatch("ApplyBrush")]
    [HarmonyPatch(new Type[] { typeof(DistrictTool.Layer), typeof(byte), typeof(float), typeof (Vector3), typeof(Vector3) })]
    public class ApplyBrush
    {
        public static void Postfix(DistrictTool.Layer layer, byte district, float brushRadius, Vector3 startPosition, Vector3 endPosition)
        {
            if (!DistrictHandler.IgnoreAll)
            {
                Command.SendToAll(new DistrictAreaModifyCommand
                {
                    Layer = layer,
                    District = district,
                    BrushRadius = brushRadius,
                    StartPosition = startPosition,
                    EndPosition = endPosition
                });
                
            }
        }
    }

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("ReleaseDistrict")]
    public class ReleaseDistrict
    {
        public static void Postfix(byte district)
        {
            if (!DistrictHandler.IgnoreAll)
            {
                Command.SendToAll(new DistrictReleaseCommand
                {
                    DistrictID = district,
                });
            }            
        }
    }

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("SetDistrictPolicy")]
    public class SetDistrictPolicy
    {
        public static void Postfix(DistrictPolicies.Policies policy, byte district)
        {
            if (!DistrictHandler.IgnoreAll)
            {
                Command.SendToAll(new DistrictPolicyCommand
                {
                    Policy = policy,
                    DistrictID = district
                });
            }
        }
    }

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("SetCityPolicy")]
    public class SetCityPolicy
    {
        public static void Postfix (DistrictPolicies.Policies policy)
        {
            if (!DistrictHandler.IgnoreAll)
            {
                Command.SendToAll(new DistrictCityPolicyCommand
                {
                    Policy = policy
                });
            }
        }
    }

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("UnsetDistrictPolicy")]
    public class UnsetDistrictPolicy
    {
        public static void Postfix (DistrictPolicies.Policies policy, byte district)
        {
            if (!DistrictHandler.IgnoreAll)
            {
                Command.SendToAll(new DistrictPolicyUnsetCommand
                {
                    Policy = policy,
                    DistrictID = district
                });
            }
        }
    }

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("UnsetCityPolicy")]
    public class UnsetCityPolicy
    {
        public static void Postfix(DistrictPolicies.Policies policy)
        {
            if (!DistrictHandler.IgnoreAll)
            {
                Command.SendToAll(new DistrictCityPolicyUnsetCommand
                {
                    Policy = policy,
                });
            }
        }
        
    }

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("CreatePark")]
    public class CreatePark
    {
        public static void Postfix(bool __result, ref byte park, DistrictPark.ParkType type, DistrictPark.ParkLevel level)
        {
            if (__result && !DistrictHandler.IgnoreAll)
            {
                Command.SendToAll(new ParkCreateCommand
                {
                    ParkID = park,
                    ParkType = type,
                    ParkLevel = level,
                });
            }
        }
    }

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("ReleasePark")]
    public class ReleasePark
    {
        public static void Prefix(ref byte park)
        {
            if (!DistrictHandler.IgnoreAll)
            {
                Command.SendToAll(new ParkReleaseCommand
                {
                    ParkID = park
                });
            }
        }
    }
}
