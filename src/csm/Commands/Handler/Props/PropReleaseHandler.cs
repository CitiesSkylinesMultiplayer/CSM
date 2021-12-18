using CSM.API.Commands;
using CSM.Commands.Data.Props;
using CSM.Helpers;

namespace CSM.Commands.Handler.Props
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
