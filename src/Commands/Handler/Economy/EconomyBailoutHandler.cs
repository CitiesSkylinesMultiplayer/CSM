using ColossalFramework.UI;
using CSM.API.Commands;
using CSM.Commands.Data.Economy;
using CSM.Helpers;
using CSM.Injections;

namespace CSM.Commands.Handler.Economy
{
    public class EconomyBailoutHandler : CommandHandler<EconomyBailoutCommand>
    {
        protected override void Handle(EconomyBailoutCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            AddResource.DontAddResource = true;

            // Runs all the required accepted/rejected code
            UIView.library.Hide(typeof(BailoutPanel).Name, command.Accepted ? 1 : 0);

            AddResource.DontAddResource = false;
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
