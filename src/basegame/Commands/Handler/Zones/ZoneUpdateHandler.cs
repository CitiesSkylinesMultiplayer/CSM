using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Zones;

namespace CSM.BaseGame.Commands.Handler.Zones
{
    public class ZoneUpdateHandler : CommandHandler<ZoneUpdateCommand>
    {
        protected override void Handle(ZoneUpdateCommand command)
        {
            ZoneManager.instance.m_blocks.m_buffer[command.ZoneId].m_zone1 = command.Zone1;
            ZoneManager.instance.m_blocks.m_buffer[command.ZoneId].m_zone2 = command.Zone2;

            IgnoreHelper.Instance.StartIgnore();
            // This Command is necessary to get the Zone to render, else it will first show when a building is build on it.
            ZoneManager.instance.m_blocks.m_buffer[command.ZoneId].RefreshZoning(command.ZoneId);
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
