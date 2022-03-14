using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Props;

namespace CSM.BaseGame.Commands.Handler.Props
{
    public class PropReleaseHandler : CommandHandler<PropReleaseCommand>
    {
        protected override void Handle(PropReleaseCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            PropManager.instance.ReleaseProp(command.PropId);
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
