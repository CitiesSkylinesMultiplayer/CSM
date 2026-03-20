using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using CSM.API;
using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Net;
using CSM.BaseGame.Helpers;

namespace CSM.BaseGame.Commands.Handler.Net
{
    public class FenceUpgradeSegmentHandler : CommandHandler<FenceUpgradeSegmentCommand>
    {
        protected override void Handle(FenceUpgradeSegmentCommand command)
        {
            NetInfo prefab = PrefabCollection<NetInfo>.GetPrefab(command.Prefab);

            NetTool tool = Singleton<ToolSimulator>.instance.GetTool<NetTool>(command.SenderId);
            ReflectionHelper.SetAttr(tool, "m_upgradedSegments", new HashSet<ushort>());
            ReflectionHelper.SetAttr(tool, "m_cachedErrors", ToolBase.ToolErrors.None);

            ReflectionHelper.SetAttr(tool, "m_upgradeSegment", command.UpgradeSegment);
            ReflectionHelper.SetAttr(tool, "m_upgradeSegmentSide", command.Side);
            tool.m_prefab = prefab;

            Type upgradeFenceMode = typeof(NetTool).GetNestedType("UpgradeFenceMode", BindingFlags.NonPublic);
            MethodInfo upgradeRoadFenceImpl = typeof(NetTool).GetMethod("UpgradeRoadFenceImpl", BindingFlags.NonPublic | BindingFlags.Instance);
            object fenceMode = Enum.ToObject(upgradeFenceMode, command.Mode);
            if (upgradeRoadFenceImpl == null)
            {
                Log.Error("Failed to find UpgradeRoadFenceImpl method!");
                return;
            }

            IgnoreHelper.Instance.StartIgnore();
            upgradeRoadFenceImpl.Invoke(tool, new[] { fenceMode });
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
