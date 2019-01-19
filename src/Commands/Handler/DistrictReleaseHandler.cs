using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class DistrictReleaseHandler : CommandHandler<DistrictReleaseCommand>
    {
        public override void Handle(DistrictReleaseCommand command)
        {
            DistrictHandler.IgnoreDistricts.Add(command.DistrictID);
            DistrictManager.instance.ReleaseDistrict(command.DistrictID);
            DistrictHandler.IgnoreDistricts.Remove(command.DistrictID);
        }

    }
}
