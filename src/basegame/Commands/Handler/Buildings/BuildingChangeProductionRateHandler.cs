using System;
using ColossalFramework.UI;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Buildings;
using CSM.BaseGame.Helpers;

namespace CSM.BaseGame.Commands.Handler.Buildings
{
    public class BuildingChangeProductionRateHandler : CommandHandler<BuildingChangeProductionRateCommand>
    {
        private static readonly Type[] checkTypes =
        {
            typeof(CityServiceWorldInfoPanel), typeof(ChirpXPanel),
            typeof(FestivalPanel), typeof(FootballPanel), typeof(VarsitySportsArenaPanel), typeof(ShelterWorldInfoPanel),
            typeof(UniqueFactoryWorldInfoPanel), typeof(WarehouseWorldInfoPanel)
        };

        protected override void Handle(BuildingChangeProductionRateCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            ref Building building = ref BuildingManager.instance.m_buildings.m_buffer[command.Building];
            building.Info.m_buildingAI.SetProductionRate(command.Building, ref building, command.Rate);

            // Update window if opened
            foreach (Type t in checkTypes)
            {
                if (InfoPanelHelper.IsBuilding(t, command.Building, out WorldInfoPanel panel))
                {
                    UICheckBox onOff = ReflectionHelper.GetAttr<UICheckBox>(panel, "m_OnOff");
                    bool isEnabled = (command.Rate != 0);

                    UISlider slider = null;
                    if (t == typeof(UniqueFactoryWorldInfoPanel) && isEnabled)
                        slider = ReflectionHelper.GetAttr<UISlider>(panel, "m_productionSlider");

                    SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                    {
                        onOff.isChecked = isEnabled;

                        if (slider != null)
                            slider.value = command.Rate;
                    });

                    break;
                }
            }

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
