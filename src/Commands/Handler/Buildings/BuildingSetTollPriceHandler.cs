using ColossalFramework.UI;
using CSM.API.Commands;
using CSM.Commands.Data.Buildings;
using CSM.Helpers;

namespace CSM.Commands.Handler.Buildings
{
    public class BuildingSetTollPriceHandler : CommandHandler<BuildingSetTollPriceCommand>
    {
        protected override void Handle(BuildingSetTollPriceCommand command)
        {
            IgnoreHelper.StartIgnore();

            ref Building building = ref BuildingManager.instance.m_buildings.m_buffer[command.Building];
            ((TollBoothAI)building.Info.m_buildingAI).SetTollPrice(command.Building, ref building, command.Price);

            if (InfoPanelHelper.IsBuilding(typeof(CityServiceWorldInfoPanel), command.Building, out WorldInfoPanel panel))
            {
                UISlider slider = ReflectionHelper.GetAttr<UISlider>(panel, "m_TicketPriceSlider");
                SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                {
                    slider.value = command.Price;
                });
            }

            IgnoreHelper.EndIgnore();
        }
    }
}
