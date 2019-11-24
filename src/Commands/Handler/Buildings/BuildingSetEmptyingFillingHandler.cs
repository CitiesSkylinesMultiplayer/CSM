using System;
using ColossalFramework.UI;
using CSM.Commands.Data.Buildings;
using CSM.Helpers;

namespace CSM.Commands.Handler.Buildings
{
    public class BuildingSetEmptyingFillingHandler : CommandHandler<BuildingSetEmptyingFillingCommand>
    {
        protected override void Handle(BuildingSetEmptyingFillingCommand command)
        {
            IgnoreHelper.StartIgnore();

            ref Building building = ref BuildingManager.instance.m_buildings.m_buffer[command.Building];
            if (command.SetEmptying)
                building.Info.m_buildingAI.SetEmptying(command.Building, ref building, command.Value);
            else
                building.Info.m_buildingAI.SetFilling(command.Building, ref building, command.Value);

            // Update panel if opened
            foreach (Type panelType in new Type[] {typeof(CityServiceWorldInfoPanel), typeof(ShelterWorldInfoPanel), typeof(WarehouseWorldInfoPanel)})
            {
                if (InfoPanelHelper.IsBuilding(panelType, command.Building, out WorldInfoPanel panel))
                {
                    SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                    {
                        if (panelType == typeof(CityServiceWorldInfoPanel))
                        {
                            // Update emptying button
                            ((CityServiceWorldInfoPanel) panel).SetSpecialActionButtonImages();

                            // Update "accept intercity trains" checkbox
                            ReflectionHelper.Call(panel, "Update");
                        }
                        else if (panelType == typeof(ShelterWorldInfoPanel))
                        {
                            ReflectionHelper.Call(panel, "RefreshEvacButton");
                        }
                        else if (panelType == typeof(WarehouseWorldInfoPanel))
                        {
                            int warehouseMode = ReflectionHelper.GetProp<int>(panel, "warehouseMode");
                            ReflectionHelper.GetAttr<UIDropDown>(panel, "m_dropdownMode").selectedIndex = warehouseMode;
                        }
                    });
                    break;
                }
            }

            IgnoreHelper.EndIgnore();
        }
    }
}
