using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Trees;
using CSM.BaseGame.Injections;

namespace CSM.BaseGame.Commands.Handler.Trees
{
    public class TreeCreateHandler : CommandHandler<TreeCreateCommand>
    {
        protected override void Handle(TreeCreateCommand command)
        {
            TreeInfo info = PrefabCollection<TreeInfo>.GetPrefab(command.InfoIndex);

            IgnoreHelper.Instance.StartIgnore();
            ArrayHandler.StartApplying(null, new uint[] { command.TreeId });

            TreeManager.instance.CreateTree(out uint _, ref SimulationManager.instance.m_randomizer, info, command.Position, command.Single);
            ArrayHandler.StopApplying();
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
