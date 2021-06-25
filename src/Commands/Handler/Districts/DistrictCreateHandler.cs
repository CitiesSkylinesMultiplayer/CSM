using CSM.API.Commands;
using CSM.Commands.Data.Districts;
using CSM.Helpers;
using CSM.Panels;
using CSM.Util;

namespace CSM.Commands.Handler.Districts
{
    public class DistrictCreateHandler : CommandHandler<DistrictCreateCommand>
    {
        protected override void Handle(DistrictCreateCommand command)
        {
            IgnoreHelper.StartIgnore();
            DistrictManager.instance.CreateDistrict(out byte district);

            if (district != command.DistrictId)
            {
                Log.Error($"District array no longer in sync! Generated {district} instead of {command.DistrictId}");
                ChatLogPanel.PrintGameMessage(ChatLogPanel.MessageType.Error, "District array no longer in sync! Please restart the multiplayer session!");
            }

            DistrictManager.instance.m_districts.m_buffer[district].m_randomSeed = command.Seed;
            IgnoreHelper.EndIgnore();
        }
    }
}
