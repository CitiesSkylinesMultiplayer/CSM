using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Districts;

namespace CSM.BaseGame.Commands.Handler.Districts
{
    public class DistrictAreaModifyHandler : CommandHandler<DistrictAreaModifyCommand>
    {
        protected override void Handle(DistrictAreaModifyCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            DistrictTool.ApplyBrush(command.Layer, command.District, command.BrushRadius, command.StartPosition, command.EndPosition);
            DistrictManager.instance.NamesModified();
            DistrictManager.instance.ParkNamesModified();

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}