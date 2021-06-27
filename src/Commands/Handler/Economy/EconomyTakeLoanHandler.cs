using CSM.API.Commands;
using CSM.Commands.Data.Economy;
using CSM.Helpers;
using CSM.Injections;

namespace CSM.Commands.Handler.Economy
{
    public class EconomyTakeLoanHandler : CommandHandler<EconomyTakeLoanCommand>
    {
        protected override void Handle(EconomyTakeLoanCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            AddResource.DontAddResource = true;

            EconomyManager.instance.TakeNewLoan(command.Index, command.Amount, command.Interest, command.Length).MoveNext();

            AddResource.DontAddResource = false;

            EconomyPanel panel = typeof(ToolsModifierControl).GetField("m_EconomyPanel", ReflectionHelper.AllAccessFlags)?.GetValue(null) as EconomyPanel;
            if (panel != null)
            {
                SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() => ReflectionHelper.Call(panel, "PopulateLoansTab"));
            }

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
