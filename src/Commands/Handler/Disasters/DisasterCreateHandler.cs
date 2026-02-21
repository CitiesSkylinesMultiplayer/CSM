using CSM.Commands.Data.Disasters;
using CSM.Helpers;
using ICities;

namespace CSM.Commands.Handler.Disasters
{
    public class DisasterCreateHandler : CommandHandler<DisasterCreateCommand>
    {

        public DisasterCreateHandler()
        {
        }

        protected override void Handle(DisasterCreateCommand command)
        {
            IgnoreHelper.StartIgnore();
            DisasterSettings settings = new DisasterSettings()
            {
                type = command.Type,
                name = command.Name,
                targetX = command.TargetX,
                targetY = command.TargetY,
                targetZ = command.TargetZ,
                angle = command.Angle,
                intensity = command.Intensity
            };

            ushort local = DisasterManager.instance.m_DisasterWrapper.CreateDisaster(settings);
            DisasterHelper.receiveCreate(command.ClientId, command.Id, local);
            IgnoreHelper.EndIgnore();
        }
    }
}
