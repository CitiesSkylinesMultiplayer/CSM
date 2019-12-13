using ColossalFramework.UI;
using CSM.Commands.Data.Buildings;
using CSM.Helpers;

namespace CSM.Commands.Handler.Buildings
{
    public class BuildingSetTransferReasonHandler : CommandHandler<BuildingSetTransferReasonCommand>
    {
        protected override void Handle(BuildingSetTransferReasonCommand command)
        {
            IgnoreHelper.StartIgnore();

            ref Building building = ref BuildingManager.instance.m_buildings.m_buffer[command.Building];
            WarehouseAI ai = building.Info.m_buildingAI as WarehouseAI;

            if (ai != null)
            {
                ai.SetTransferReason(command.Building, ref building, command.Material);
            }

            if (InfoPanelHelper.IsBuilding(typeof(WarehouseWorldInfoPanel), command.Building, out WorldInfoPanel panel))
            {
                TransferManager.TransferReason[] reasons = ReflectionHelper.GetAttr<TransferManager.TransferReason[]>(panel, "m_transferReasons");
                for (int i = 0; i < reasons.Length; i++)
                {
                    if (reasons[i] == command.Material)
                    {
                        UIDropDown dropDown = ReflectionHelper.GetAttr<UIDropDown>(panel, "m_dropdownResource");
                        SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                        {
                            dropDown.selectedIndex = i;
                        });
                        break;
                    }
                }
            }

            IgnoreHelper.EndIgnore();
        }
    }
}
