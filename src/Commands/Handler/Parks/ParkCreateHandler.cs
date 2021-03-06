using CSM.Commands.Data.Parks;
using CSM.Helpers;
using CSM.Panels;
using CSM.Util;

namespace CSM.Commands.Handler.Parks
{
    public class ParkCreateHandler : CommandHandler<ParkCreateCommand>
    {
        protected override void Handle(ParkCreateCommand command)
        {
            IgnoreHelper.StartIgnore();
            DistrictManager.instance.CreatePark(out byte park, command.ParkType, command.ParkLevel);

            if (park != command.ParkId)
            {
                Log.Error($"Park array no longer in sync! Generated {park} instead of {command.ParkId}");
                ChatLogPanel.PrintGameMessage(ChatLogPanel.MessageType.Error, "Park array no longer in sync! Please restart the multiplayer session!");
            }

            DistrictManager.instance.m_parks.m_buffer[park].m_randomSeed = command.Seed;
            IgnoreHelper.EndIgnore();
        }
    }
}
