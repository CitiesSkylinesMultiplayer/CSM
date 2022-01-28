using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Parks;

namespace CSM.BaseGame.Commands.Handler.Parks
{
    public class ParkCreateHandler : CommandHandler<ParkCreateCommand>
    {
        protected override void Handle(ParkCreateCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();
            DistrictManager.instance.CreatePark(out byte park, command.ParkType, command.ParkLevel);

            if (park != command.ParkId)
            {
                Log.Error($"Park array no longer in sync! Generated {park} instead of {command.ParkId}");
                Chat.Instance.PrintGameMessage(Chat.MessageType.Error, "Park array no longer in sync! Please restart the multiplayer session!");
            }

            DistrictManager.instance.m_parks.m_buffer[park].m_randomSeed = command.Seed;
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
