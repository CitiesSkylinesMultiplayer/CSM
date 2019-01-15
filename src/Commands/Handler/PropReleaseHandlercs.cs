using CSM.Injections;
using CSM.Networking;


namespace CSM.Commands.Handler
{
    class PropReleaseHandlercs : CommandHandler<PropReleaseCommand>
    {
        public override byte ID => CommandIds.PropReleaseCommand;

        public override void HandleOnClient(PropReleaseCommand command) => Handle(command);

        public override void HandleOnServer(PropReleaseCommand command, Player player) => Handle(command);

        private void Handle(PropReleaseCommand command)
        {
            PropHandler.IgnoreProp.Add(command.PropID);
            PropManager.instance.ReleaseProp(command.PropID);
            PropHandler.IgnoreProp.Remove(command.PropID);
        }
    }
}
