using ColossalFramework;

namespace CSM.Commands.Handler
{
    public class BuildingRelocateHandler : CommandHandler<BuildingRelocateCommand>
    {
        public override void Handle(BuildingRelocateCommand command)
        {
            Singleton<BuildingManager>.instance.RelocateBuilding((ushort) command.BuidlingId, command.NewPosition, command.Angle);
        }
    }
}
