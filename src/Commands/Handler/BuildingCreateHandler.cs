using ColossalFramework;
using CSM.Extensions;
using CSM.Networking;
using UnityEngine;

namespace CSM.Commands.Handler
{
    public class BuildingCreateHandler : CommandHandler<BuildingCreateCommand>
    {
        public override byte ID => 103;

        public override void HandleOnServer(BuildingCreateCommand command, Player player) => HandleBuilding(command);

        public override void HandleOnClient(BuildingCreateCommand command) => HandleBuilding(command);

        private void HandleBuilding(BuildingCreateCommand command)
        {
            BuildingInfo info = PrefabCollection<BuildingInfo>.GetPrefab(command.Infoindex);
            BuildingExtension.LastPosition = command.Position;
            ushort building = command.BuildingID;

            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_flags = Building.Flags.Created;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].Info = info;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].Width = info.m_cellWidth;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].Length = command.Length;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_frame0 = new Building.Frame();
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_buildIndex = Singleton<SimulationManager>.instance.m_currentBuildIndex;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_angle = command.Angle;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_position = command.Position;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_baseHeight = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_ownVehicles = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_guestVehicles = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_sourceCitizens = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_targetCitizens = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_citizenUnits = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_netNode = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_subBuilding = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_parentBuilding = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_waterSource = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_eventIndex = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_nextGridBuilding = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_nextGridBuilding2 = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_electricityBuffer = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_waterBuffer = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_sewageBuffer = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_heatingBuffer = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_garbageBuffer = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_crimeBuffer = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_customBuffer1 = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_customBuffer2 = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_buildWaterHeight = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_productionRate = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_waterPollution = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_fireIntensity = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_problems = Notification.Problem.None;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_lastFrame = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_tempImport = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_tempExport = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_finalImport = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_finalExport = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_education1 = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_education2 = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_education3 = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_teens = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_youngs = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_adults = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_seniors = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_fireHazard = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_electricityProblemTimer = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_heatingProblemTimer = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_waterProblemTimer = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_workerProblemTimer = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_incomingProblemTimer = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_outgoingProblemTimer = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_healthProblemTimer = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_deathProblemTimer = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_serviceProblemTimer = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_taxProblemTimer = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_majorProblemTimer = 0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_levelUpProgress = 0;
            //Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_subCulture = 0;
            info.m_buildingAI.CreateBuilding(building, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[building]);
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_frame1 = Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_frame0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_frame2 = Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_frame0;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_frame3 = Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_frame0;
            int num = Mathf.Clamp((int)((command.Position.x / 64f) + 135f), 0, 0x10d);
            int index = (Mathf.Clamp((int)((command.Position.z / 64f) + 135f), 0, 0x10d) * 270) + num;
            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_nextGridBuilding = Singleton<BuildingManager>.instance.m_buildingGrid[index];
            Singleton<BuildingManager>.instance.m_buildingGrid[index] = building;

            Singleton<BuildingManager>.instance.UpdateBuilding(building);
            Singleton<BuildingManager>.instance.UpdateBuildingColors(building);
            Singleton<BuildingManager>.instance.m_buildingCount = ((int)Singleton<BuildingManager>.instance.m_buildings.ItemCount()) - 1;

            Singleton<SimulationManager>.instance.m_currentBuildIndex++;
        }
    }
}