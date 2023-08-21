using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Economy;
using CSM.BaseGame.Injections;

namespace CSM.BaseGame.Commands.Handler.Economy
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
                SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() => ReflectionHelper.Call(panel, "UpdateLoansTab"));
            }

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
