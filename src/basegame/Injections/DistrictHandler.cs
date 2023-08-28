using System;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Threading;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Districts;
using CSM.BaseGame.Commands.Data.Parks;
using HarmonyLib;
using UnityEngine;

namespace CSM.BaseGame.Injections
{
    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("CreateDistrict")]
    public class CreateDistrict
    {
        public static void Postfix(bool __result, ref byte district)
        {
            if (IgnoreHelper.Instance.IsIgnored())
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
    [HarmonyPatch(new Type[] { typeof(DistrictTool.Layer), typeof(byte), typeof(float), typeof(Vector3), typeof(Vector3), typeof(bool) })]
    public class ApplyBrush
    {
        public static void Postfix(DistrictTool.Layer layer, byte districtOrPark, float brushRadius, Vector3 startPosition, Vector3 endPosition)
        {
            if (!IgnoreHelper.Instance.IsIgnored())
            {
                Command.SendToAll(new DistrictAreaModifyCommand
                {
                    Layer = layer,
                    District = districtOrPark,
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
            if (!IgnoreHelper.Instance.IsIgnored())
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
            if (!IgnoreHelper.Instance.IsIgnored())
            {
                Command.SendToAll(new DistrictPolicyCommand
                {
                    Policy = policy,
                    DistrictId = district,
                    IsPark = false
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
            if (!IgnoreHelper.Instance.IsIgnored())
            {
                Command.SendToAll(new DistrictCityPolicyCommand
                {
                    Policy = policy
                });
            }
        }
    }

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("SetParkPolicy")]
    public class SetParkPolicy
    {
        public static void Postfix(DistrictPolicies.Policies policy, byte park)
        {
            if (!IgnoreHelper.Instance.IsIgnored())
            {
                Command.SendToAll(new DistrictPolicyCommand
                {
                    Policy = policy,
                    DistrictId = park,
                    IsPark = true
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
            if (!IgnoreHelper.Instance.IsIgnored())
            {
                Command.SendToAll(new DistrictPolicyUnsetCommand
                {
                    Policy = policy,
                    DistrictId = district,
                    IsPark = false
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
            if (!IgnoreHelper.Instance.IsIgnored())
            {
                Command.SendToAll(new DistrictCityPolicyUnsetCommand
                {
                    Policy = policy,
                });
            }
        }
    }

    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("UnsetParkPolicy")]
    public class UnsetParkPolicy
    {
        public static void Postfix(DistrictPolicies.Policies policy, byte park)
        {
            if (!IgnoreHelper.Instance.IsIgnored())
            {
                Command.SendToAll(new DistrictPolicyUnsetCommand
                {
                    Policy = policy,
                    DistrictId = park,
                    IsPark = true
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
            if (__result && !IgnoreHelper.Instance.IsIgnored())
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
            if (!IgnoreHelper.Instance.IsIgnored())
            {
                Command.SendToAll(new ParkReleaseCommand
                {
                    ParkId = park
                });
            }
        }
    }

    [HarmonyPatch(typeof(DistrictWorldInfoPanel))]
    [HarmonyPatch("OnStyleChanged")]
    public class StyleChanged
    {
        public static void Prefix(DistrictWorldInfoPanel __instance, int value)
        {
            byte district = ReflectionHelper.GetAttr<InstanceID>(__instance, "m_InstanceID").District;
            ushort oldStyle = Singleton<DistrictManager>.instance.m_districts.m_buffer[district].m_Style;
            if (oldStyle != value)
            {
                Command.SendToAll(new DistrictChangeStyleCommand
                {
                    Style = (ushort) value,
                    DistrictId = district
                });
            }
        }
    }

    // Fix exception caused by cities in some cases in this method:
    [HarmonyPatch(typeof(DistrictWorldInfoPanel))]
    [HarmonyPatch("OnPoliciesChanged")]
    public class OnPoliciesChanged
    {
        public static bool Prefix(DistrictWorldInfoPanel __instance)
        {
            MethodInfo updatePolicies = typeof(DistrictWorldInfoPanel).GetMethod("UpdatePolicies", ReflectionHelper.AllAccessFlags);

            // If we are already in the correct thread, call directly
            if (ThreadHelper.dispatcher == Dispatcher.currentSafe)
            {
                updatePolicies.Invoke(__instance, new object[0]);
            }
            else
            {
                ThreadHelper.dispatcher.Dispatch(() => updatePolicies.Invoke(__instance, new object[0]));
            }

            return false;
        }
    }
}
