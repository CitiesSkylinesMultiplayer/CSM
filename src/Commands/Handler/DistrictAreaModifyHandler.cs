using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class DistrictAreaModifyHandler : CommandHandler<DistrictAreaModifyCommand>
    {
        public override void Handle(DistrictAreaModifyCommand command)
        {
            DistrictHandler.IgnoreAll = true;

            DistrictTool.ApplyBrush(command.Layer, command.District, command.BrushRadius, command.StartPosition, command.EndPosition);
            DistrictManager.instance.NamesModified();
            DistrictManager.instance.ParkNamesModified();

            DistrictHandler.IgnoreAll = false;
        }
    }
}
