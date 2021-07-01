using CSM.Commands;
using CSM.Commands.Data.Names;
using CSM.Commands.Data.Net;
using CSM.Commands.Handler.Names;
using CSM.Helpers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSM.Injections
{
    public static class NameHandler
    {
        public static bool CanRun(object instance, bool state)
        {
            if (IgnoreHelper.Instance.IsIgnored() || !state)
                return false;

            return ReflectionHelper.GetAttr<bool>(instance, "$current");
        }

        public static void CheckCounter(object instance, out bool state)
        {
            int counter = ReflectionHelper.GetAttr<int>(instance, "$PC");
            state = counter == 0;
        }
    }

    [HarmonyPatch(typeof(NetSegment))]
    [HarmonyPatch("UpdateNameSeed")]
    public class UpdateSegmentSeed
    {
        public static void Prefix(out ushort __state, ushort ___m_nameSeed)
        {
            __state = ___m_nameSeed; // Seed before method
        }

        public static void Postfix(ref ushort __state, ushort ___m_nameSeed, ushort segmentID)
        {
            // Check if seed was changed
            if (__state != ___m_nameSeed)
            {
                Command.SendToAll(new SegmentSeedUpdateCommand()
                {
                    SegmentId = segmentID,
                    NameSeed = ___m_nameSeed
                });
            }
        }
    }

    [HarmonyPatch]
    public class SetBuildingName
    {
        public static void Prefix(ushort ___building, string ___name, object __instance, out bool __state)
        {
            NameHandler.CheckCounter(__instance, out __state);
            if (__state && BuildingManager.instance.GetBuildingName(___building, InstanceID.Empty) == ___name)
                __state = false;
        }

        public static void Postfix(ushort ___building, string ___name, ref bool __state, object __instance)
        {
            if (!NameHandler.CanRun(__instance, __state))
                return;

            Command.SendToAll(new ChangeNameCommand
            {
                Type = InstanceType.Building,
                Id = ___building,
                Name = ___name
            });
        }

        public static MethodBase TargetMethod()
        {
            return ReflectionHelper.GetIteratorTargetMethod(typeof(BuildingManager), "<SetBuildingName>c__Iterator2", out Type _);
        }
    }

    [HarmonyPatch]
    public class SetCitizenName
    {
        public static void Prefix(uint ___citizenID, string ___name, object __instance, out bool __state)
        {
            NameHandler.CheckCounter(__instance, out __state);
            if (__state && CitizenManager.instance.GetCitizenName(___citizenID) == ___name)
                __state = false;
        }

        public static void Postfix(uint ___citizenID, string ___name, ref bool __state, object __instance)
        {
            if (!NameHandler.CanRun(__instance, __state))
                return;

            Command.SendToAll(new ChangeNameCommand
            {
                Type = InstanceType.Citizen,
                Id = (int)___citizenID,
                Name = ___name
            });
        }

        public static MethodBase TargetMethod()
        {
            return ReflectionHelper.GetIteratorTargetMethod(typeof(CitizenManager), "<SetCitizenName>c__Iterator0", out Type _);
        }
    }

    [HarmonyPatch]
    public class SetInstanceName
    {
        public static void Prefix(ushort ___instanceID, string ___name, object __instance, out bool __state)
        {
            NameHandler.CheckCounter(__instance, out __state);
            if (__state && CitizenManager.instance.GetInstanceName(___instanceID) == ___name)
                __state = false;
        }

        public static void Postfix(ushort ___instanceID, string ___name, ref bool __state, object __instance)
        {
            if (!NameHandler.CanRun(__instance, __state))
                return;

            Command.SendToAll(new ChangeNameCommand
            {
                Type = InstanceType.CitizenInstance,
                Id = ___instanceID,
                Name = ___name
            });
        }

        public static MethodBase TargetMethod()
        {
            return ReflectionHelper.GetIteratorTargetMethod(typeof(CitizenManager), "<SetInstanceName>c__Iterator1", out Type _);
        }
    }

    [HarmonyPatch]
    public class SetDisasterName
    {
        public static void Prefix(ushort ___disasterID, string ___name, object __instance, out bool __state)
        {
            NameHandler.CheckCounter(__instance, out __state);
            if (__state && DisasterManager.instance.GetDisasterName(___disasterID) == ___name)
                __state = false;
        }

        public static void Postfix(ushort ___disasterID, string ___name, ref bool __state, object __instance)
        {
            if (!NameHandler.CanRun(__instance, __state))
                return;

            Command.SendToAll(new ChangeNameCommand
            {
                Type = InstanceType.Disaster,
                Id = ___disasterID,
                Name = ___name
            });
        }

        public static MethodBase TargetMethod()
        {
            return ReflectionHelper.GetIteratorTargetMethod(typeof(DisasterManager), "<SetDisasterName>c__Iterator0", out Type _);
        }
    }

    [HarmonyPatch]
    public class SetDistrictName
    {
        public static void Prefix(int ___district, string ___name, object __instance, out bool __state)
        {
            NameHandler.CheckCounter(__instance, out __state);
            if (__state && DistrictManager.instance.GetDistrictName(___district) == ___name)
                __state = false;
        }

        public static void Postfix(int ___district, string ___name, ref bool __state, object __instance)
        {
            if (!NameHandler.CanRun(__instance, __state))
                return;

            Command.SendToAll(new ChangeNameCommand
            {
                Type = InstanceType.District,
                Id = ___district,
                Name = ___name
            });
        }

        public static MethodBase TargetMethod()
        {
            return ReflectionHelper.GetIteratorTargetMethod(typeof(DistrictManager), "<SetDistrictName>c__Iterator0", out Type _);
        }
    }

    [HarmonyPatch]
    public class SetParkName
    {
        public static void Prefix(int ___park, string ___name, object __instance, out bool __state)
        {
            NameHandler.CheckCounter(__instance, out __state);
            if (__state && DistrictManager.instance.GetParkName(___park) == ___name)
                __state = false;
        }

        public static void Postfix(int ___park, string ___name, ref bool __state, object __instance)
        {
            if (!NameHandler.CanRun(__instance, __state))
                return;

            Command.SendToAll(new ChangeNameCommand
            {
                Type = InstanceType.Park,
                Id = ___park,
                Name = ___name
            });
        }

        public static MethodBase TargetMethod()
        {
            return ReflectionHelper.GetIteratorTargetMethod(typeof(DistrictManager), "<SetParkName>c__Iterator1", out Type _);
        }
    }

    [HarmonyPatch(typeof(EventManager))]
    [HarmonyPatch("SetEventNameImpl")]
    public class SetEventName
    {
        public static void Prefix(ushort eventID, string name, out bool __state)
        {
            __state = EventManager.instance.GetEventName(eventID) != name;
        }

        public static void Postfix(ushort eventID, string name, bool __result, ref bool __state)
        {
            if (IgnoreHelper.Instance.IsIgnored() || !__state)
                return;

            if (!__result)
                return;

            Command.SendToAll(new ChangeNameCommand
            {
                Type = InstanceType.Event,
                Id = eventID,
                Name = name
            });
        }
    }

    [HarmonyPatch]
    public class SetSegmentName
    {
        public static void Prefix(ushort ___segmentID, string ___name, object __instance, out bool __state)
        {
            NameHandler.CheckCounter(__instance, out __state);
            if (__state && NetManager.instance.GetSegmentName(___segmentID) == ___name)
                __state = false;
        }

        public static void Postfix(IEnumerator<bool> __instance, ref bool __state, ushort ___segmentID, string ___name)
        {
            if (!NameHandler.CanRun(__instance, __state))
                return;

            Command.SendToAll(new ChangeNameCommand
            {
                Type = InstanceType.NetSegment,
                Id = ___segmentID,
                Name = ___name
            });
        }

        public static MethodBase TargetMethod()
        {
            return ReflectionHelper.GetIteratorTargetMethod(typeof(NetManager), "<SetSegmentName>c__Iterator2", out Type _);
        }
    }

    [HarmonyPatch]
    public class SetLineName
    {
        public static void Prefix(ushort ___lineID, string ___name, object __instance, out bool __state)
        {
            NameHandler.CheckCounter(__instance, out __state);
            if (__state && TransportManager.instance.GetLineName(___lineID) == ___name)
                __state = false;
        }

        public static void Postfix(ushort ___lineID, string ___name, ref bool __state, object __instance)
        {
            if (!NameHandler.CanRun(__instance, __state))
                return;

            Command.SendToAll(new ChangeNameCommand
            {
                Type = InstanceType.TransportLine,
                Id = ___lineID,
                Name = ___name
            });
        }

        public static MethodBase TargetMethod()
        {
            return ReflectionHelper.GetIteratorTargetMethod(typeof(TransportManager), "<SetLineName>c__Iterator1", out Type _);
        }
    }

    [HarmonyPatch]
    public class SetVehicleName
    {
        public static void Prefix(ushort ___vehicleID, string ___name, object __instance, out bool __state)
        {
            NameHandler.CheckCounter(__instance, out __state);
            if (__state && VehicleManager.instance.GetVehicleName(___vehicleID) == ___name)
                __state = false;
        }

        public static void Postfix(ushort ___vehicleID, string ___name, ref bool __state, object __instance)
        {
            if (!NameHandler.CanRun(__instance, __state))
                return;

            Command.SendToAll(new ChangeNameCommand
            {
                Type = InstanceType.Vehicle,
                Id = ___vehicleID,
                Name = ___name
            });
        }

        public static MethodBase TargetMethod()
        {
            return ReflectionHelper.GetIteratorTargetMethod(typeof(VehicleManager), "<SetVehicleName>c__Iterator2", out Type _);
        }
    }

    [HarmonyPatch]
    public class SetParkedVehicleName
    {
        public static void Prefix(ushort ___parkedID, string ___name, object __instance, out bool __state)
        {
            NameHandler.CheckCounter(__instance, out __state);
            if (__state && VehicleManager.instance.GetParkedVehicleName(___parkedID) == ___name)
                __state = false;
        }

        public static void Postfix(ushort ___parkedID, string ___name, ref bool __state, object __instance)
        {
            if (!NameHandler.CanRun(__instance, __state))
                return;

            Command.SendToAll(new ChangeNameCommand
            {
                Type = InstanceType.ParkedVehicle,
                Id = ___parkedID,
                Name = ___name
            });
        }

        public static MethodBase TargetMethod()
        {
            return ReflectionHelper.GetIteratorTargetMethod(typeof(VehicleManager), "<SetParkedVehicleName>c__Iterator3", out Type _);
        }
    }

    [HarmonyPatch]
    public class SetCityName
    {
        public static void Prefix(object __instance, out bool __state)
        {
            NameHandler.CheckCounter(__instance, out __state);
        }

        public static void Postfix(string ___name, ref bool __state)
        {
            if (IgnoreHelper.Instance.IsIgnored() || !__state)
                return;

            Command.SendToAll(new ChangeCityNameCommand
            {
                Name = ___name
            });
        }

        public static MethodBase TargetMethod()
        {
            return ReflectionHelper.GetIteratorTargetMethod(typeof(CityInfoPanel), "<SetCityName>c__Iterator1", out Type _);
        }
    }

    [HarmonyPatch(typeof(InfoPanel))]
    [HarmonyPatch("Awake")]
    public class InfoPanelAwake
    {
        // There doesn't seem to be any way to get the instance of the InfoPanel
        // so we save it at the beginning
        public static void Postfix(InfoPanel __instance)
        {
            ChangeCityNameHandler.Panel = __instance;
        }
    }
}
