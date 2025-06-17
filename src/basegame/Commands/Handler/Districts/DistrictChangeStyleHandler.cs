using System;
using ColossalFramework;
using ColossalFramework.UI;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Districts;
using CSM.BaseGame.Helpers;

namespace CSM.BaseGame.Commands.Handler.Districts
{
    public class DistrictChangeStyleHandler : CommandHandler<DistrictChangeStyleCommand>
    {
        protected override void Handle(DistrictChangeStyleCommand command)
        {
            Singleton<DistrictManager>.instance.m_districts.m_buffer[command.DistrictId].m_Style = command.Style;

            if (InfoPanelHelper.IsDistrict(typeof(DistrictWorldInfoPanel), command.DistrictId, out WorldInfoPanel pan))
            {
                SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                {
                    DistrictWorldInfoPanel panel = (DistrictWorldInfoPanel) pan;
                    int[] styleMap = ReflectionHelper.GetAttr<int[]>(panel, "m_StyleMap");
                    int num = Array.IndexOf(styleMap, command.Style);
                    ReflectionHelper.GetAttr<UIDropDown>(panel, "m_Style").selectedIndex = num;
                });
            }
        }
    }
}
