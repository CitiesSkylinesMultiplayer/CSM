using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Campus;
using UnityEngine;

namespace CSM.BaseGame.Commands.Handler.Campus
{
    public class OnAcademicYearEndHandler : CommandHandler<OnAcademicYearEndCommand>
    {
        protected override void Handle(OnAcademicYearEndCommand command)
        {
            // This is a reproduction of the effects of DistrictPark::OnAcademicYearEnded
            ref DistrictPark park = ref Singleton<DistrictManager>.instance.m_parks.m_buffer[command.ParkId];
            park.m_flags &= ~DistrictPark.Flags.Graduation;
            park.m_awayMatchesDone = null;
            park.m_academicWorksData = command.AcademicWorksData;
            ReflectionHelper.SetAttr(park.m_ledger, "m_reports", command.LedgerReports);
            park.m_grantType = 0;
            park.m_dynamicVarsityAttractivenessModifier = command.DynamicAttractiveness;
            park.m_academicStaffAccumulation = 0.0f;
            if (park.m_sports != DistrictPark.Sports.None)
            {
                park.m_lifetimeTrophies += park.m_ledger.ReadTrophyCount(DistrictPark.AcademicYear.Last);
            }
            Singleton<MessageManager>.instance.TryCreateMessage("GRADUATIONCHIRP_GENERIC", null, Singleton<MessageManager>.instance.GetRandomResidentID());

            // Now the content of DistrictPark::RefreshCampusLevel
            if (command.NewParkLevel > command.OldParkLevel)
            {
                for (DistrictPark.ParkLevel newLevel = command.OldParkLevel + 1; newLevel <= command.NewParkLevel; ++newLevel)
                    Singleton<DistrictManager>.instance.SetParkTypeLevel(command.ParkId, park.m_parkType, newLevel);
            }
            else
            {
                if (command.OldParkLevel >= DistrictPark.ParkLevel.Level4 && command.NewParkLevel <= DistrictPark.ParkLevel.Level3 &&
                    Singleton<DistrictManager>.instance.IsParkPolicySet(DistrictPolicies.Policies.SponsorshipDeal, command.ParkId))
                    Singleton<DistrictManager>.instance.UnsetParkPolicy(DistrictPolicies.Policies.SponsorshipDeal, command.ParkId);
                Singleton<DistrictManager>.instance.SetParkTypeLevel(command.ParkId, park.m_parkType, command.NewParkLevel);
            }
            Singleton<DistrictManager>.instance.ParkNamesModified();
            if (command.NewParkLevel == DistrictPark.ParkLevel.Level5 && Singleton<SimulationManager>.instance.m_metaData.m_disableAchievements != SimulationMetaData.MetaBool.True)
                ThreadHelper.dispatcher.Dispatch(() => SteamHelper.UnlockAchievement("DistinguishedAcademics"));
            if (command.NewParkLevel != command.OldParkLevel)
            {
                FastList<ushort> serviceBuildings = Singleton<BuildingManager>.instance.GetServiceBuildings(ItemClass.Service.PlayerEducation);
                for (int index1 = 0; index1 < serviceBuildings.m_size; ++index1)
                {
                    ushort index2 = serviceBuildings.m_buffer[index1];
                    if (Singleton<DistrictManager>.instance.GetPark(Singleton<BuildingManager>.instance.m_buildings.m_buffer[index2].m_position) == command.ParkId)
                    {
                        BuildingInfo info = Singleton<BuildingManager>.instance.m_buildings.m_buffer[index2].Info;
                        if (info != null && (info.m_buildingAI is MainCampusBuildingAI || info.m_buildingAI is CampusBuildingAI))
                        {
                            Vector3 position = Singleton<BuildingManager>.instance.m_buildings.m_buffer[index2].m_position;
                            position.y += info.m_size.y;
                            Singleton<NotificationManager>.instance.AddEvent(command.NewParkLevel >= command.OldParkLevel ? NotificationEvent.Type.PlayerLevelUp : NotificationEvent.Type.LoseHappiness, position, 1f);
                        }
                    }
                }
            }

            if (Singleton<DistrictManager>.instance.m_properties != null)
            {
                EffectInfo campusLevelupEffect = Singleton<DistrictManager>.instance.m_properties.m_campusLevelupEffect;

                if (campusLevelupEffect != null)
                {
                    InstanceID instance3 = new InstanceID();
                    instance3.Park = command.ParkId;
                    EffectInfo.SpawnArea spawnArea = new EffectInfo.SpawnArea(park.m_nameLocation, Vector3.up, 100f);
                    Singleton<EffectManager>.instance.DispatchEffect(campusLevelupEffect, instance3, spawnArea,
                        Vector3.zero, 0.0f, 1f, Singleton<AudioManager>.instance.DefaultGroup);
                }
            }

            // Refresh panels
            ThreadHelper.dispatcher.Dispatch(() =>
            {
                List<byte> activeAreas =
                    ReflectionHelper.GetAttr<List<byte>>(typeof(DistrictManager), "m_activeCampusAreas");
                UIView.library.Get<AcademicYearReportPanel>("AcademicYearReportPanel").PopupPanel(activeAreas, 0, false);
                CampusWorldInfoPanel campusWorldInfoPanel = UIView.library.Get<CampusWorldInfoPanel>("CampusWorldInfoPanel");
                if (!campusWorldInfoPanel.component.isVisible)
                    return;
                campusWorldInfoPanel.OnAcademicYearEnded();
            });
        }
    }
}
