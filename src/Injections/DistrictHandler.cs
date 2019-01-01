using ColossalFramework;
using CSM.Commands;
using Harmony;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static DistrictTool;

namespace CSM.Injections
{
    class DistrictHandler
    {
        public static List<ushort> IgnoreDistricts { get; } = new List<ushort>();
        public static List<Vector3> IgnoreAreaModified { get; } = new List<Vector3>();
    }


    [HarmonyPatch(typeof(DistrictManager))]
    [HarmonyPatch("CreateDistrict")]
    public class CreateDistrict
    {
        public static void Postfix(bool __result, ref byte district)
        {
            if (__result && !DistrictHandler.IgnoreDistricts.Contains(district))
            {
                Command.SendToAll(new DistrictCreateCommand
                {
                    DistrictID = district
                });
            }
        }
    }

    [HarmonyPatch(typeof(DistrictTool))]
    [HarmonyPatch("ApplyBrush")]
    [HarmonyPatch(new Type[] { typeof(Layer), typeof(byte), typeof(float), typeof (Vector3), typeof(Vector3)})]
    public class ApplyBrush
    {
        public static void Postfix (Layer layer, byte district, float brushRadius, Vector3 startPosition, Vector3 endPosition)
        {
            if (!DistrictHandler.IgnoreAreaModified.Contains(startPosition))
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
            if (!DistrictHandler.IgnoreDistricts.Contains(district))
            {
                Command.SendToAll(new DistrictReleaseCommand
                {
                    DistrictID = district,
                });
            }
        }
    }


}
