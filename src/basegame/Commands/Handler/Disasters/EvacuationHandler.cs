using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Disasters;
using ColossalFramework;
using HarmonyLib;

namespace CSM.BaseGame.Commands.Handler.Disasters
{
    public class EvacuationHandler : CommandHandler<EvacuationCommand>
    {
        protected override void Handle(EvacuationCommand command)
        {
            if (command.BuildingID == 0)
                return;

            BuildingManager instance = Singleton<BuildingManager>.instance;
            BuildingInfo info = instance.m_buildings.m_buffer[command.BuildingID].Info;

            if (info == null || info.m_buildingAI == null)
                return;

            Log.Info($"[CSM] Setting evacuation for building {command.BuildingID} ({info.name}) to {command.Evacuating}");

            IgnoreHelper.Instance.StartIgnore();

            // Use Harmony's AccessTools to call the protected SetEvacuating method
            var method = AccessTools.Method(typeof(CommonBuildingAI), "SetEvacuating", new[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(bool) });
            method.Invoke(info.m_buildingAI, new object[] { command.BuildingID, instance.m_buildings.m_buffer[command.BuildingID], command.Evacuating });

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
