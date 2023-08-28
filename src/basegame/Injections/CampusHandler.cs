using System;
using ColossalFramework;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Campus;
using HarmonyLib;
using UnityEngine;

namespace CSM.BaseGame.Injections
{
    [HarmonyPatch(typeof(CampusWorldInfoPanel))]
    [HarmonyPatch("OnBuyResearchGrant")]
    public class OnBuyResearchGrant
    {
        public static void Postfix(InstanceID ___m_InstanceID)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            byte park = ___m_InstanceID.Park;
            byte grantType = Singleton<DistrictManager>.instance.m_parks.m_buffer[park].m_grantType;
            Command.SendToAll(new BuyResearchGrantCommand
            {
                Campus = park,
                GrantType = grantType
            });
        }
    }

    [HarmonyPatch(typeof(CampusWorldInfoPanel))]
    [HarmonyPatch("OnAcademicStaffValueChanged")]
    public class OnAcademicStaffValueChanged
    {
        public static void Prefix(InstanceID ___m_InstanceID, float value)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            byte park = ___m_InstanceID.Park;
            if (Singleton<DistrictManager>.instance.m_parks.m_buffer[park].m_academicStaffCount != (byte)value)
            {
                Command.SendToAll(new SetAcademicStaffCountCommand
                {
                    Campus = park,
                    Count = (byte) value
                });
            }
        }
    }

    [HarmonyPatch(typeof(CampusWorldInfoPanel))]
    [HarmonyPatch("OnCoachesCountChanged")]
    public class OnCoachesCountChanged
    {
        public static void Prefix(InstanceID ___m_InstanceID, float value)
        {
            byte park = ___m_InstanceID.Park;
            _sendPacket = Singleton<DistrictManager>.instance.m_parks.m_buffer[park].m_academicStaffCount !=
                          (byte)value;
        }

        public static void Postfix(InstanceID ___m_InstanceID, float value)
        {
            if (!_sendPacket || IgnoreHelper.Instance.IsIgnored())
                return;

            byte park = ___m_InstanceID.Park;
            DateTime[] coachHireTimes = Singleton<DistrictManager>.instance.m_parks.m_buffer[park].m_coachHireTimes;
            Command.SendToAll(new SetCoachesCountCommand
            {
                Campus = park,
                Count = (byte) value,
                CoachHireTimes = coachHireTimes
            });
        }

        private static bool _sendPacket;
    }

    [HarmonyPatch(typeof(CampusWorldInfoPanel))]
    [HarmonyPatch("OnCheerleadingBudgetChanged")]
    public class OnCheerleadingBudgetChanged
    {
        public static void Prefix(InstanceID ___m_InstanceID, float value)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            byte park = ___m_InstanceID.Park;
            if (Singleton<DistrictManager>.instance.m_parks.m_buffer[park].m_cheerleadingBudget != (int)value)
            {
                Command.SendToAll(new SetCheerleadingBudgetCommand
                {
                    Campus = park,
                    Budget = (int)value
                });
            }
        }
    }

    [HarmonyPatch(typeof(CampusWorldInfoPanel))]
    [HarmonyPatch("OnTicketPriceChanged")]
    public class OnTicketPriceChanged
    {
        public static void Prefix(InstanceID ___m_InstanceID, float value)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            byte park = ___m_InstanceID.Park;
            ushort newPrice = (ushort)(value * 100.0);
            if (Singleton<DistrictManager>.instance.m_parks.m_buffer[park].m_ticketPrice != newPrice)
            {
                Command.SendToAll(new SetTicketPriceCommand
                {
                    Campus = park,
                    Price = newPrice
                });
            }
        }
    }

    [HarmonyPatch(typeof(CampusWorldInfoPanel))]
    [HarmonyPatch("OnVarsityIdentityChanged")]
    public class OnVarsityIdentityChanged
    {
        public static void Prefix(InstanceID ___m_InstanceID, int value)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            byte park = ___m_InstanceID.Park;
            if (Singleton<DistrictManager>.instance.m_parks.m_buffer[park].m_varsityIdentityIndex != value)
            {
                Command.SendToAll(new SetVarsityIdentityCommand
                {
                    Campus = park,
                    Identity = value
                });
            }
        }
    }

    [HarmonyPatch(typeof(CampusWorldInfoPanel))]
    [HarmonyPatch("OnVarsityColorChanged")]
    public class OnVarsityColorChanged
    {
        public static void Prefix(InstanceID ___m_InstanceID, Color value)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            byte park = ___m_InstanceID.Park;
            if (Singleton<DistrictManager>.instance.m_parks.m_buffer[park].m_varsityColor != value)
            {
                Command.SendToAll(new SetVarsityColorCommand
                {
                    Campus = park,
                    Color = value
                });
            }
        }
    }

    [HarmonyPatch(typeof(CampusWorldInfoPanel))]
    [HarmonyPatch("OnSetTarget")]
    public class OnSetTarget
    {
        public static void Prefix()
        {
            IgnoreHelper.Instance.StartIgnore();
        }

        public static void Postfix()
        {
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
