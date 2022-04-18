using System.Collections.Generic;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Trees;
using CSM.BaseGame.Helpers;
using ColossalFramework;

namespace CSM.BaseGame.Commands.Handler.Trees
{
    public class TreeUpgradeSegmentHandler : CommandHandler<TreeUpgradeSegmentCommand>
    {
        protected override void Handle(TreeUpgradeSegmentCommand command)
        {
            TreeInfo prefab = PrefabCollection<TreeInfo>.GetPrefab(command.Prefab);

            TreeTool tool = Singleton<ToolSimulator>.instance.GetTool<TreeTool>(command.SenderId);
            ReflectionHelper.SetAttr(tool, "m_upgradedSegments", new HashSet<ushort>());
            ReflectionHelper.SetAttr(tool, "m_placementErrors", ToolBase.ToolErrors.None);

            ReflectionHelper.SetAttr(tool, "m_upgradeSegment", command.UpgradeSegment);
            ReflectionHelper.SetAttr(tool, "m_mousePosition", command.MousePosition);
            tool.m_prefab = prefab;

            IgnoreHelper.Instance.StartIgnore();
            ReflectionHelper.Call(tool, "UpgradeSegmentImpl");
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
