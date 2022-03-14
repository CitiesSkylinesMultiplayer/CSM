using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Economy;

namespace CSM.BaseGame.Commands.Handler.Economy
{
    public class EconomySetTaxRateHandler : CommandHandler<EconomySetTaxRateCommand>
    {
        protected override void Handle(EconomySetTaxRateCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            EconomyManager.instance.SetTaxRate(command.Service, command.SubService, command.Level, command.Rate);

            EconomyPanel panel = typeof(ToolsModifierControl).GetField("m_EconomyPanel", ReflectionHelper.AllAccessFlags)?.GetValue(null) as EconomyPanel;
            if (panel != null)
            {
                SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() => ReflectionHelper.Call(panel, "PopulateTaxesTab"));
            }

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
