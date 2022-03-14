using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Economy;
using CSM.BaseGame.Injections;

namespace CSM.BaseGame.Commands.Handler.Economy
{
    public class EconomyPayLoanHandler : CommandHandler<EconomyPayLoanCommand>
    {
        protected override void Handle(EconomyPayLoanCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            FetchResource.DontFetchResource = true;
            FetchResource.ReturnFetchedAmount = command.Paid;

            EconomyManager.instance.PayLoanNow(command.Index).MoveNext();

            FetchResource.DontFetchResource = false;

            EconomyPanel panel = typeof(ToolsModifierControl).GetField("m_EconomyPanel", ReflectionHelper.AllAccessFlags)?.GetValue(null) as EconomyPanel;
            if (panel != null)
            {
                SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() => ReflectionHelper.Call(panel, "PopulateLoansTab"));
            }

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
