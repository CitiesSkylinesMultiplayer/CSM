using CSM.API.Commands;
using CSM.BaseGame.Commands.Data.Buildings;
using CSM.BaseGame.Helpers;

namespace CSM.BaseGame.Commands.Handler.Buildings
{
    public class BuildingStatusHandler : CommandHandler<BuildingStatusCommand>
    {
        public BuildingStatusHandler()
        {
            RelayOnServer = false;
        }
        protected override void Handle(BuildingStatusCommand command)
        {
            BuildingStatusSync.Receive(command);
        }
    }

    public class RequestBuildingStatusHandler : CommandHandler<RequestBuildingStatusCommand>
    {
        public RequestBuildingStatusHandler()
        {
            RelayOnServer = false;
        }
        protected override void Handle(RequestBuildingStatusCommand command)
        {
            BuildingStatusSync.Answer(command);
        }
    }
}
