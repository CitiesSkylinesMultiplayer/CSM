using CSM.API.Commands;
using CSM.Commands.Data.Net;
using CSM.Helpers;
using CSM.Injections;

namespace CSM.Commands.Handler.Net
{
    public class NodeCreateHandler : CommandHandler<NodeCreateCommand>
    {
        protected override void Handle(NodeCreateCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            ArrayHandler.StartApplying(command.Array16Ids, command.Array32Ids);

            NetInfo prefab = PrefabCollection<NetInfo>.GetPrefab(command.Prefab);

            FastList<NetTool.NodePosition> nodeBuffer = new FastList<NetTool.NodePosition>();

            NetTool.CreateNode(prefab, command.StartPoint, command.MiddlePoint, command.EndPoint, nodeBuffer,
                command.MaxSegments, false, command.TestEnds, false, command.AutoFix, false,
                command.Invert, command.SwitchDir, command.RelocateBuildingId, out ushort _,
                out ushort _, out ushort _, out int _, out int _);

            ArrayHandler.StopApplying();
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
