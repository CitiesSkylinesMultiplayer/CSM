using CSM.Commands.Data.Buildings;
using CSM.Helpers;
using CSM.Injections;
using System;

namespace CSM.Commands.Handler.Buildings
{
    public class BuildingRebuildHandler : CommandHandler<BuildingRebuildCommand>
    {
        private static object rebuildClickedDelegate;
        private static Type delegateType = null;

        protected override void Handle(BuildingRebuildCommand command)
        {
            IgnoreHelper.StartIgnore();

            // Using a delegate object because the 'OnRebuildClicked' delegate contains most of the needed code
            // This code from the CityServiceWorldInfoPanel is the same as in the EventBuildingWorldInfoPanel,
            // UniqueFactoryWorldInfoPanel and WarehouseWorldInfoPanel
            if (delegateType == null)
            {
                delegateType = typeof(CityServiceWorldInfoPanel).GetNestedType("<OnRebuildClicked>c__AnonStorey2", ReflectionHelper.AllAccessFlags);
                rebuildClickedDelegate = Activator.CreateInstance(delegateType);
            }

            ReflectionHelper.SetAttr(rebuildClickedDelegate, "buildingID", command.Building);

            ArrayHandler.StartApplying(command.Array16Ids, null);

            FetchResource.DontFetchResource = true;

            delegateType.GetMethod("<>m__0", ReflectionHelper.AllAccessFlags)?.Invoke(rebuildClickedDelegate, null);

            FetchResource.DontFetchResource = false;

            ArrayHandler.StopApplying();

            IgnoreHelper.EndIgnore();
        }
    }
}
