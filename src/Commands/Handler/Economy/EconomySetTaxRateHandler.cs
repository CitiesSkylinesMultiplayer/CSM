using CSM.Commands.Data.Economy;
using CSM.Helpers;

namespace CSM.Commands.Handler.Economy
{
    public class EconomySetTaxRateHandler : CommandHandler<EconomySetTaxRateCommand>
    {
        protected override void Handle(EconomySetTaxRateCommand command)
        {
            IgnoreHelper.StartIgnore();

            EconomyManager.instance.SetTaxRate(command.Service, command.SubService, command.Level, command.Rate);

            EconomyPanel panel = typeof(ToolsModifierControl).GetField("m_EconomyPanel", ReflectionHelper.AllAccessFlags)?.GetValue(null) as EconomyPanel;
            if (panel != null)
            {
                SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() => ReflectionHelper.Call(panel, "PopulateTaxesTab"));
            }

            IgnoreHelper.EndIgnore();
        }
    }
}
