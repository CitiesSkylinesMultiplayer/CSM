using System;
using CSM.Commands.Data.Buildings;
using CSM.Helpers;
using CSM.Injections;

namespace CSM.Commands.Handler.Buildings
{
    public class BuildingUpdateFlagsHandler : CommandHandler<BuildingUpdateFlagsCommand>
    {
        protected override void Handle(BuildingUpdateFlagsCommand command)
        {
            IgnoreHelper.StartIgnore();

            BuildingManager.instance.UpdateFlags(command.Building, command.ChangeMask);

            IgnoreHelper.EndIgnore();
        }
    }
}
