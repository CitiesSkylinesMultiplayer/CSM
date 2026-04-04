using System;
using System.Collections.Generic;
using System.Reflection;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Disasters;
using HarmonyLib;

namespace CSM.BaseGame.Injections.Buildings
{
    [HarmonyPatch]
    public class BuildingAIHandler
    {
        public static void Prefix(ushort buildingID, ref Building data, bool evacuating)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            bool isEvacuating = (data.m_flags & Building.Flags.Evacuating) != Building.Flags.None;
            if (isEvacuating == evacuating)
                return;

            Command.SendToAll(new EvacuationCommand
            {
                BuildingID = buildingID,
                Evacuating = evacuating
            });
        }

        public static IEnumerable<MethodBase> TargetMethods()
        {
            Type[] targets = { typeof(CommonBuildingAI), typeof(BuildingAI), typeof(ShelterAI) };
            foreach (var type in targets)
            {
                var method = type.GetMethod("SetEvacuating", ReflectionHelper.AllAccessFlags);
                if (method != null)
                {
                    Log.Info($"[CSM] Patching SetEvacuating on {type.Name}");
                    yield return method;
                }
            }
        }
    }
}
