using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Props;
using CSM.BaseGame.Injections;

namespace CSM.BaseGame.Commands.Handler.Props
{
    public class PropCreateHandler : CommandHandler<PropCreateCommand>
    {
        protected override void Handle(PropCreateCommand command)
        {
            PropInfo info = PrefabCollection<PropInfo>.GetPrefab(command.InfoIndex);

            IgnoreHelper.Instance.StartIgnore();
            ArrayHandler.StartApplying(new ushort[] { command.PropId }, null);

            PropManager.instance.CreateProp(out ushort _, ref SimulationManager.instance.m_randomizer, info, command.Position, command.Angle, command.Single);

            ArrayHandler.StopApplying();
            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
