using CSM.Injections;
using CSM.Networking;


namespace CSM.Commands.Handler
{
    public class DistrictCreateHandler : CommandHandler<DistrictCreateCommand>
    {
        public override byte ID => CommandIds.DistrictCreateCommand;

        public override void HandleOnClient(DistrictCreateCommand command) => Handle(command);

        public override void HandleOnServer(DistrictCreateCommand command, Player player) => Handle(command);

        private void Handle(DistrictCreateCommand command)
        {
            DistrictHandler.IgnoreDistricts.Add(command.DistrictID);
            DistrictManager.instance.CreateDistrict(out byte district);
            DistrictHandler.IgnoreDistricts.Remove(command.DistrictID);
            UnityEngine.Debug.Log($"district: {district}");
        }
    }
}
