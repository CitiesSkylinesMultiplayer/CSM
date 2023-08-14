﻿using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Net;
using CSM.BaseGame.Injections;

namespace CSM.BaseGame.Commands.Handler.Net
{
    public class NodeCreateHandler : CommandHandler<NodeCreateCommand>
    {
        protected override void Handle(NodeCreateCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            ArrayHandler.StartApplying(command.Array16Ids, command.Array32Ids);

            NetInfo prefab = PrefabCollection<NetInfo>.GetPrefab(command.Prefab);

            FastList<NetTool.NodePosition> nodeBuffer = new FastList<NetTool.NodePosition>();

            NetSegment.Flags2 oldZoneGridFlags = NetTool.m_zoneGridFlags;
            NetTool.m_zoneGridFlags = command.ZoneGridFlags;

            NetTool.CreateNode(prefab, command.StartPoint, command.MiddlePoint, command.EndPoint, nodeBuffer,
                command.MaxSegments, false, command.TestEnds, false, command.AutoFix, false,
                command.Invert, command.SwitchDir, command.RelocateBuildingId, out ushort _,
                out ushort _, out ushort _, out int _, out int _);

            NetTool.m_zoneGridFlags = oldZoneGridFlags;

            ArrayHandler.StopApplying();
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
