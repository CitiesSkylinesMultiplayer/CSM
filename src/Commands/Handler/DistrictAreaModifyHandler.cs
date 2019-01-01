using CSM.Injections;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class DistrictAreaModifyHandler : CommandHandler<DistrictAreaModifyCommand>
    {
        public override byte ID => CommandIds.DistrictAreaModifyCommand;

        public override void HandleOnClient(DistrictAreaModifyCommand command) => Handle(command);

        public override void HandleOnServer(DistrictAreaModifyCommand command, Player player) => Handle(command);

        private void Handle(DistrictAreaModifyCommand command)
        {
            DistrictHandler.IgnoreAreaModified.Add(command.StartPosition);
            DistrictTool.ApplyBrush(command.Layer, command.District, command.BrushRadius, command.StartPosition, command.EndPosition);
            DistrictHandler.IgnoreAreaModified.Remove(command.StartPosition);
        }

    }
}
