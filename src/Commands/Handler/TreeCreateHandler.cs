using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class TreeCreateHandler : CommandHandler<TreeCreateCommand>
    {
        public override void Handle(TreeCreateCommand command)
        {
            TreeInfo info = PrefabCollection<TreeInfo>.GetPrefab(command.InfoIndex);

            TreeHandler.IgnoreAll = true;
            ArrayHandler.StartApplying(null, new uint[] { command.TreeID });

            TreeManager.instance.CreateTree(out uint _, ref SimulationManager.instance.m_randomizer, info, command.Position, command.Single);
            ArrayHandler.StopApplying();
            TreeHandler.IgnoreAll = false;
        }
    }
}
