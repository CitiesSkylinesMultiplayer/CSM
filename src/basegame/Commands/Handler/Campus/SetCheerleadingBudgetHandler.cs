using ColossalFramework;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Campus;
using CSM.BaseGame.Helpers;

namespace CSM.BaseGame.Commands.Handler.Campus
{
    public class SetCheerleadingBudgetHandler : CommandHandler<SetCheerleadingBudgetCommand>
    {
        protected override void Handle(SetCheerleadingBudgetCommand command)
        {
            int oldBonus = Singleton<DistrictManager>.instance.m_parks.m_buffer[command.Campus].CalculateCheerleadingAttractivenessBonus();

            Singleton<DistrictManager>.instance.m_parks.m_buffer[command.Campus].m_cheerleadingBudget = command.Budget;

            int newBonus = Singleton<DistrictManager>.instance.m_parks.m_buffer[command.Campus].CalculateCheerleadingAttractivenessBonus();
            int bonus = newBonus - oldBonus;
            Singleton<DistrictManager>.instance.m_parks.m_buffer[command.Campus].ModifyStaticAttractiveness(bonus);

            // Refresh panel
            if (InfoPanelHelper.IsPark(typeof(CampusWorldInfoPanel), command.Campus, out WorldInfoPanel panel))
            {
                SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                {
                    ReflectionHelper.Call((CampusWorldInfoPanel)panel, "OnSetTarget");
                });
            }
        }
    }
}
