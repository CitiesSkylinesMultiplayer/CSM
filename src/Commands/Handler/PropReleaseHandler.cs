using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class PropReleaseHandler : CommandHandler<PropReleaseCommand>
    {
        public override void Handle(PropReleaseCommand command)
        {
            PropHandler.IgnoreProp.Add(command.PropID);
            PropManager.instance.ReleaseProp(command.PropID);
            PropHandler.IgnoreProp.Remove(command.PropID);
        }
    }
}
