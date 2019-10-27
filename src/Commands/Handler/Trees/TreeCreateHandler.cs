using CSM.Commands.Data.Trees;
using CSM.Helpers;
using CSM.Injections;

namespace CSM.Commands.Handler.Trees
{
    public class TreeCreateHandler : CommandHandler<TreeCreateCommand>
    {
        protected override void Handle(TreeCreateCommand command)
        {
            TreeInfo info = PrefabCollection<TreeInfo>.GetPrefab(command.InfoIndex);

            IgnoreHelper.StartIgnore();
            ArrayHandler.StartApplying(null, new uint[] { command.TreeId });

            TreeManager.instance.CreateTree(out uint _, ref SimulationManager.instance.m_randomizer, info, command.Position, command.Single);
            ArrayHandler.StopApplying();
            IgnoreHelper.EndIgnore();
        }
    }
}
