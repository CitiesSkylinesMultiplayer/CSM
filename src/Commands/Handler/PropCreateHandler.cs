using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class PropCreateHandler : CommandHandler<PropCreateCommand>
    {
        public override void Handle(PropCreateCommand command)
        {
            PropInfo info = PrefabCollection<PropInfo>.GetPrefab(command.infoindex);

            PropHandler.IgnoreAll = true;
            ArrayHandler.StartApplying(new ushort[] { command.PropID }, null);

            PropManager.instance.CreateProp(out ushort _, ref SimulationManager.instance.m_randomizer, info, command.position, command.angle, command.single);

            ArrayHandler.StopApplying();
            PropHandler.IgnoreAll = false;
        }
    }
}
