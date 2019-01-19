using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class DistrictAreaModifyHandler : CommandHandler<DistrictAreaModifyCommand>
    {
        public override void Handle(DistrictAreaModifyCommand command)
        {
            DistrictHandler.IgnoreAreaModified.Add(command.StartPosition);
            DistrictTool.ApplyBrush(command.Layer, command.District, command.BrushRadius, command.StartPosition, command.EndPosition);
            DistrictManager.instance.NamesModified();
            DistrictManager.instance.ParkNamesModified();
            DistrictHandler.IgnoreAreaModified.Remove(command.StartPosition);
        }

    }
}
