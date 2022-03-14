using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Economy;

namespace CSM.BaseGame.Commands.Handler.Economy
{
    public class EconomySetBudgetHandler : CommandHandler<EconomySetBudgetCommand>
    {
        protected override void Handle(EconomySetBudgetCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            EconomyManager.instance.SetBudget(command.Service, command.SubService, command.Budget, command.Night);

            EconomyPanel panel = typeof(ToolsModifierControl).GetField("m_EconomyPanel", ReflectionHelper.AllAccessFlags)?.GetValue(null) as EconomyPanel;
            if (panel != null)
            {
                SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() => ReflectionHelper.Call(panel, "PopulateBudgetTab"));
            }

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
