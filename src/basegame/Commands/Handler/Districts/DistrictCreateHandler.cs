using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Districts;

namespace CSM.BaseGame.Commands.Handler.Districts
{
    public class DistrictCreateHandler : CommandHandler<DistrictCreateCommand>
    {
        protected override void Handle(DistrictCreateCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            DistrictManager.instance.CreateDistrict(out byte district);

            if (district != command.DistrictId)
            {
                Log.Error($"District array no longer in sync! Generated {district} instead of {command.DistrictId}");
                Chat.Instance.PrintGameMessage(Chat.MessageType.Error, "District array no longer in sync! Please restart the multiplayer session!");
            }

            DistrictManager.instance.m_districts.m_buffer[district].m_randomSeed = command.Seed;
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
