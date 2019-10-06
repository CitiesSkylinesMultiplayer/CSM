using System.Collections;
using CSM.Common;
using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class BuildingToolCreateHandler : CommandHandler<BuildingToolCreateCommand>
    {
        public override void Handle(BuildingToolCreateCommand command)
        {
            BuildingTool tool = ToolSimulator.GetTool<BuildingTool>(command.SenderId);
            
            NetHandler.IgnoreAll = true;
            TreeHandler.IgnoreAll = true;
            PropHandler.IgnoreAll = true;
            BuildingHandler.IgnoreAll = true;
            ArrayHandler.StartApplying(command.Array16Ids, command.Array32Ids);
            
            BuildingInfo prefab = null;
            if (command.Relocate == 0)
                prefab = PrefabCollection<BuildingInfo>.GetPrefab(command.Prefab);

            tool.m_prefab = prefab;
            tool.m_relocate = command.Relocate;

            ReflectionHelper.SetAttr(tool, "m_mousePosition", command.MousePosition);
            ReflectionHelper.SetAttr(tool, "m_mouseAngle", command.MouseAngle);
            ReflectionHelper.SetAttr(tool, "m_elevation", command.Elevation);
            ReflectionHelper.SetAttr(tool, "m_placementErrors", ToolBase.ToolErrors.None);
            ReflectionHelper.SetAttr(tool, "m_constructionCost", 0);

            ToolController controller = ReflectionHelper.GetAttr<ToolController>(tool, "m_toolController");
            ReflectionHelper.SetAttr(controller, "m_collidingSegments1", command.CollidingSegments);
            ReflectionHelper.SetAttr(controller, "m_collidingBuildings1", command.CollidingBuildings);
            ReflectionHelper.SetAttr(controller, "m_collidingDepth", 1);
            
            ReflectionHelper.Call<IEnumerator>(tool, "CreateBuilding")?.MoveNext();

            ArrayHandler.StopApplying();
            BuildingHandler.IgnoreAll = false;
            PropHandler.IgnoreAll = false;
            TreeHandler.IgnoreAll = false;
            NetHandler.IgnoreAll = false;
        }
    }
}
