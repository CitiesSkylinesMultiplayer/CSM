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

            if (InfoPanelHelper.IsDistrict(typeof(DistrictWorldInfoPanel), command.DistrictId, out WorldInfoPanel panel))
            {
                SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                {
                    ReflectionHelper.GetAttr<UIDropDown>((DistrictWorldInfoPanel)panel, "m_Style").selectedIndex = command.Style;
                });
            }
        }
    }
}
