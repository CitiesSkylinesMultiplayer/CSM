using ICities;

namespace CSM.Extensions
{
    public class BuildingExtension : BuildingExtensionBase
    {
        public override void OnCreated(IBuilding building)
        {
        }

        public override void OnBuildingCreated(ushort id)
        {
            base.OnBuildingCreated(id);
        }

        public override void OnBuildingReleased(ushort id)
        {
            base.OnBuildingReleased(id);
        }

        public override void OnBuildingRelocated(ushort id)
        {
            base.OnBuildingRelocated(id);
        }
    }
}