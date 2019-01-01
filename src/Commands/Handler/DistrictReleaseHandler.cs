using CSM.Injections;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class DistrictReleaseHandler : CommandHandler<DistrictReleaseCommand>
    {
        public override byte ID => CommandIds.DistrictReleaseCommand;

        public override void HandleOnClient(DistrictReleaseCommand command) => Handle(command);

        public override void HandleOnServer(DistrictReleaseCommand command, Player player) => Handle(command);

        private void Handle(DistrictReleaseCommand command)
        {
            DistrictHandler.IgnoreDistricts.Add(command.DistrictID);
            DistrictManager.instance.ReleaseDistrict(command.DistrictID);
            DistrictHandler.IgnoreDistricts.Remove(command.DistrictID);
        }

    }
}
