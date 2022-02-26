using CSM.Commands.Data.Game;
using CSM.Helpers;

namespace CSM.Commands.Handler.Disasters
{
    public class DisasterStartHandler : CommandHandler<DisasterStartCommand>
    {

        public DisasterCreateHandler()
        {
        }

        protected override void Handle(DisasterStartCommand command)
        {
            IgnoreHelper.StartIgnore();
            ushort local = DisablerHelper.getLocal(command.ClientId, command.Id);
            DisasterManager.instance.m_DisasterWrapper.StartDisaster(local);
            IgnoreHelper.EndIgnore();
        }
    }
}
