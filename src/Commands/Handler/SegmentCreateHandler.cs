using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class SegmentCreateHandler : CommandHandler<SegmentCreateCommand>
    {
        public override void Handle(SegmentCreateCommand command)
        {
            NetInfo info = PrefabCollection<NetInfo>.GetPrefab(command.InfoIndex);

            NodeHandler.IgnoreAll = true;
            ArrayHandler.StartApplying(new ushort[] { command.SegmentId }, null);

            NetManager.instance.CreateSegment(out ushort _, ref SimulationManager.instance.m_randomizer, info, command.StartNode, command.EndNode, command.StartDirection, command.EndDirection, SimulationManager.instance.m_currentBuildIndex++, command.ModifiedIndex, command.Invert);

            ArrayHandler.StopApplying();
            NodeHandler.IgnoreAll = false;
        }
    }
}
