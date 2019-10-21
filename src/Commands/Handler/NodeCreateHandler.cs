using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class NodeCreateHandler : CommandHandler<NodeCreateCommand>
    {
        public override void Handle(NodeCreateCommand command)
        {
            NetHandler.IgnoreAll = true;
            TreeHandler.IgnoreAll = true;
            PropHandler.IgnoreAll = true;
            BuildingHandler.IgnoreAll = true;

            ArrayHandler.StartApplying(command.Array16Ids, command.Array32Ids);

            NetInfo prefab = PrefabCollection<NetInfo>.GetPrefab(command.Prefab);
            
            FastList<NetTool.NodePosition> nodeBuffer = new FastList<NetTool.NodePosition>();

            NetTool.CreateNode(prefab, command.StartPoint, command.MiddlePoint, command.EndPoint, nodeBuffer,
                command.MaxSegments, false, command.TestEnds, false, command.AutoFix, false,
                command.Invert, command.SwitchDir, command.RelocateBuildingId, out ushort _,
                out ushort _, out ushort _, out int _, out int _);

            ArrayHandler.StopApplying();
            BuildingHandler.IgnoreAll = false;
            PropHandler.IgnoreAll = false;
            TreeHandler.IgnoreAll = false;
            NetHandler.IgnoreAll = false;
        }
    }
}
