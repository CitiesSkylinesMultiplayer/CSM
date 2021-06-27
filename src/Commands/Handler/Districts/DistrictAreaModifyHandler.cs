using CSM.API.Commands;
using CSM.Commands.Data.Districts;
using CSM.Helpers;

namespace CSM.Commands.Handler.Districts
{
    public class DistrictAreaModifyHandler : CommandHandler<DistrictAreaModifyCommand>
    {
        protected override void Handle(DistrictAreaModifyCommand command)
        {
            IgnoreHelper.StartIgnore();

            DistrictTool.ApplyBrush(command.Layer, command.District, command.BrushRadius, command.StartPosition, command.EndPosition);
            DistrictManager.instance.NamesModified();
            DistrictManager.instance.ParkNamesModified();

            IgnoreHelper.EndIgnore();
        }
    }
}