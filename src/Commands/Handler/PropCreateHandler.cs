using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class PropCreateHandler : CommandHandler<PropCreateCommand>
    {
        public override void Handle(PropCreateCommand command)
        {
            PropInfo info = PrefabCollection<PropInfo>.GetPrefab(command.InfoIndex);

            PropHandler.IgnoreAll = true;
            ArrayHandler.StartApplying(new ushort[] { command.PropId }, null);

            PropManager.instance.CreateProp(out ushort _, ref SimulationManager.instance.m_randomizer, info, command.Position, command.Angle, command.Single);

            ArrayHandler.StopApplying();
            PropHandler.IgnoreAll = false;
        }
    }
}
