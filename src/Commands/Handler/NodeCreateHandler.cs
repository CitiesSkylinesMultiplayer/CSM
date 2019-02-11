using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class NodeCreateHandler : CommandHandler<NodeCreateCommand>
    {
        public override void Handle(NodeCreateCommand command)
        {
            NetInfo info = PrefabCollection<NetInfo>.GetPrefab(command.InfoIndex);

            NodeHandler.IgnoreAll = true;
            ArrayHandler.StartApplying(new ushort[] { command.NodeId }, null);

            NetManager.instance.CreateNode(out ushort _, ref SimulationManager.instance.m_randomizer, info, command.Position, SimulationManager.instance.m_currentBuildIndex++);

            ArrayHandler.StopApplying();
            NodeHandler.IgnoreAll = false;
        }
    }
}
