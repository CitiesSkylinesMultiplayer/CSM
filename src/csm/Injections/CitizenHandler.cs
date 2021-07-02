using CSM.Networking;
using HarmonyLib;
using System;
using ColossalFramework.Math;

namespace CSM.Injections
{
    
    [HarmonyPatch(typeof(CitizenManager))]
    [HarmonyPatch("CreateUnits")]
    // public bool CreateUnits(out uint firstUnit, ref Randomizer randomizer, ushort building, ushort vehicle, int homeCount, int workCount, int visitCount, int passengerCount, int studentCount)
    public class CreateUnits
    {
        
        public static bool Prefix(ref bool __result)
        {
            if (MultiplayerManager.Instance.CurrentRole != MultiplayerRole.Server)
            {
                __result = true;
                return false;
            }
            return true;
        }
        
    }
    
    [HarmonyPatch(typeof(CitizenManager))]
    [HarmonyPatch("CreateCitizen")]
    // public bool CreateCitizen(out uint citizen, int age, int family, ref Randomizer r)
    public class CreateCitizenWithoutGender
    {
        
        public static bool Prefix(ref bool __result)
        {
            if (MultiplayerManager.Instance.CurrentRole != MultiplayerRole.Server)
            {
                __result = true;
                return false;
            }
            return true;
        }
        
    }
    
    [HarmonyPatch(typeof(CitizenManager))]
    [HarmonyPatch("CreateCitizen")]
    [HarmonyPatch(new Type[] { typeof(uint), typeof(int), typeof(int), typeof(Randomizer), typeof(Citizen.Gender) })]
    // public bool CreateCitizen(out uint citizen, int age, int family, ref Randomizer r, Citizen.Gender gender)
    public class CreateCitizenWithGender
    {
        
        public static bool Prefix(ref bool __result)
        {
            if (MultiplayerManager.Instance.CurrentRole != MultiplayerRole.Server)
            {
                __result = true;
                return false;
            }
            return true;
        }
        
    }
    
    [HarmonyPatch(typeof(CitizenManager))]
    [HarmonyPatch("CreateCitizenInstance")]
    [HarmonyPatch(new Type[] { typeof(ushort), typeof(Randomizer), typeof(CitizenInfo), typeof(uint) })]
    // public bool CreateCitizenInstance( out ushort instance, ref Randomizer randomizer, CitizenInfo info, uint citizen)
    public class CreateCitizenInstance
    {
        
        public static bool Prefix(ref bool __result)
        {
            if (MultiplayerManager.Instance.CurrentRole != MultiplayerRole.Server)
            {
                __result = true;
                return false;
            }
            return true;
        }
        
    }
    
}