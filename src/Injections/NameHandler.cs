using CSM.Commands;
using Harmony;
using System;
using System.Reflection;
using CSM.Commands.Handler;
using CSM.Common;

namespace CSM.Injections
{
    /*
     * TODO: Initially sync road name seeds
     * The seed is set after CreateSegment is called (in NetTool)
     * To solve this, we can either sync all NetTool calls instead of the NetManager
     * or send another command at the end of the tick after a segment has been created
     */
    public static class NameHandler
    {
        public static bool IgnoreAll { get; set; } = false;

        public static bool CanRun(object instance, bool state)
        {
            if (IgnoreAll || !state)
                return false;

            return ReflectionHelper.GetAttr<bool>(instance, "$current");
        }

        public static void CheckCounter(object instance, ref bool state)
        {
            int counter = ReflectionHelper.GetAttr<int>(instance, "$PC");
            state = counter == 0;
        }
    }
    [HarmonyPatch]
    public class SetBuildingName
    {
        public static void Prefix(object __instance, ref bool __state)
        {
            NameHandler.CheckCounter(__instance, ref __state);
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
        public static void Prefix(object __instance, ref bool __state)
        {
            NameHandler.CheckCounter(__instance, ref __state);
        }
        
        public static void Postfix(uint ___citizenID, string ___name, ref bool __state, object __instance)
        {
            if (!NameHandler.CanRun(__instance, __state))
                return;

            Command.SendToAll(new ChangeNameCommand
            {
                Type = InstanceType.Citizen,
                Id = (int) ___citizenID,
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
        public static void Prefix(object __instance, ref bool __state)
        {
            NameHandler.CheckCounter(__instance, ref __state);
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
        public static void Prefix(object __instance, ref bool __state)
        {
            NameHandler.CheckCounter(__instance, ref __state);
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
        public static void Prefix(object __instance, ref bool __state)
        {
            NameHandler.CheckCounter(__instance, ref __state);
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
        public static void Prefix(object __instance, ref bool __state)
        {
            NameHandler.CheckCounter(__instance, ref __state);
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
        public static void Postfix(ushort eventID, string name, bool __result)
        {
            if (NameHandler.IgnoreAll)
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
    
    [HarmonyPatch(typeof(NetManager))]
    [HarmonyPatch("SetSegmentNameImpl")]
    public class SetSegmentName
    {
        public static void Postfix(ushort segmentID, string name, bool __result)
        {
            if (NameHandler.IgnoreAll)
                return;

            if (!__result)
                return;

            Command.SendToAll(new ChangeNameCommand
            {
                Type = InstanceType.NetSegment,
                Id = segmentID,
                Name = name
            });
        }
    }
    
    [HarmonyPatch]
    public class SetLineName
    {
        public static void Prefix(object __instance, ref bool __state)
        {
            NameHandler.CheckCounter(__instance, ref __state);
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
        public static void Prefix(object __instance, ref bool __state)
        {
            NameHandler.CheckCounter(__instance, ref __state);
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
        public static void Prefix(object __instance, ref bool __state)
        {
            NameHandler.CheckCounter(__instance, ref __state);
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
        public static void Prefix(object __instance, ref bool __state)
        {
            NameHandler.CheckCounter(__instance, ref __state);
        }
        
        public static void Postfix(string ___name, ref bool __state)
        {
            if (NameHandler.IgnoreAll || !__state)
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
