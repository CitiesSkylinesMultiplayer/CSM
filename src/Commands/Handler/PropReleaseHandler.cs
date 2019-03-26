using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class PropReleaseHandler : CommandHandler<PropReleaseCommand>
    {
        public override void Handle(PropReleaseCommand command)
        {
            PropHandler.IgnoreAll = true;
            PropManager.instance.ReleaseProp(command.PropID);
            PropHandler.IgnoreAll = false;
        }
    }
}
