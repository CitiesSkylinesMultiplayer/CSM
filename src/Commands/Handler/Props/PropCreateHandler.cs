using CSM.API.Commands;
using CSM.Commands.Data.Props;
using CSM.Helpers;
using CSM.Injections;

namespace CSM.Commands.Handler.Props
{
    public class PropCreateHandler : CommandHandler<PropCreateCommand>
    {
        protected override void Handle(PropCreateCommand command)
        {
            PropInfo info = PrefabCollection<PropInfo>.GetPrefab(command.InfoIndex);

            IgnoreHelper.StartIgnore();
            ArrayHandler.StartApplying(new ushort[] { command.PropId }, null);

            PropManager.instance.CreateProp(out ushort _, ref SimulationManager.instance.m_randomizer, info, command.Position, command.Angle, command.Single);

            ArrayHandler.StopApplying();
            IgnoreHelper.EndIgnore();
        }
    }
}
