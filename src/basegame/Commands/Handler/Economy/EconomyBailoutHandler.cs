using ColossalFramework.UI;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Economy;
using CSM.BaseGame.Injections;

namespace CSM.BaseGame.Commands.Handler.Economy
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
