﻿using CSM.Commands;
using CSM.Commands.Data.Districts;
using CSM.Commands.Data.Parks;
using CSM.Helpers;
using HarmonyLib;
using System;
using UnityEngine;

namespace CSM.Injections
{
    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("CreateDistrict")]
    public class CreateDistrict
    {
        public static void Postfix(bool __result, ref byte district)
        {
            if (IgnoreHelper.IsIgnored())
                return;

            if (__result)
            {
                ulong seed = DistrictManager.instance.m_districts.m_buffer[district].m_randomSeed;

                Command.SendToAll(new DistrictCreateCommand
                {
                    DistrictId = district,
                    Seed = seed
                });
            }
        }
    }

    [HarmonyPatch(typeof(DistrictTool))]
    [HarmonyPatch("ApplyBrush")]
    [HarmonyPatch(new Type[] { typeof(DistrictTool.Layer), typeof(byte), typeof(float), typeof(Vector3), typeof(Vector3) })]
    public class ApplyBrush
    {
        public static void Postfix(DistrictTool.Layer layer, byte district, float brushRadius, Vector3 startPosition, Vector3 endPosition)
        {
            if (!IgnoreHelper.IsIgnored())
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
            if (!IgnoreHelper.IsIgnored())
            {
                Command.SendToAll(new DistrictReleaseCommand
                {
                    DistrictId = district,
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
            if (!IgnoreHelper.IsIgnored())
            {
                Command.SendToAll(new DistrictPolicyCommand
                {
                    Policy = policy,
                    DistrictId = district
                });
            }
        }
    }

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("SetCityPolicy")]
    public class SetCityPolicy
    {
        public static void Postfix(DistrictPolicies.Policies policy)
        {
            if (!IgnoreHelper.IsIgnored())
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
        public static void Postfix(DistrictPolicies.Policies policy, byte district)
        {
            if (!IgnoreHelper.IsIgnored())
            {
                Command.SendToAll(new DistrictPolicyUnsetCommand
                {
                    Policy = policy,
                    DistrictId = district
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
            if (!IgnoreHelper.IsIgnored())
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
            if (__result && !IgnoreHelper.IsIgnored())
            {
                ulong seed = DistrictManager.instance.m_parks.m_buffer[park].m_randomSeed;

                Command.SendToAll(new ParkCreateCommand
                {
                    ParkId = park,
                    ParkType = type,
                    ParkLevel = level,
                    Seed = seed
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
            if (!IgnoreHelper.IsIgnored())
            {
                Command.SendToAll(new ParkReleaseCommand
                {
                    ParkId = park
                });
            }
        }
    }
}
