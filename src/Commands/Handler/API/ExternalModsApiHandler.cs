using CSM.API.Commands;
using CSM.Commands.Data.API;
using CSM.Helpers;
using CSM.Mods;

namespace CSM.Commands.Handler.API
{
    class ExternalModsApiHandler : CommandHandler<ExternalAPICommand>
    {

        protected override void Handle(ExternalAPICommand command)
        {
            IgnoreHelper.StartIgnore();
            ModSupport.SendCommandToLocalMod(command);
            
            IgnoreHelper.EndIgnore();
        }
    }
}
