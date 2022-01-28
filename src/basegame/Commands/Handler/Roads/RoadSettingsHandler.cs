using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Roads;

namespace CSM.BaseGame.Commands.Handler.Roads
{
    public class RoadSettingsHandler : CommandHandler<RoadSettingsCommand>
    {
        protected override void Handle(RoadSettingsCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            // Simulate traffic routes mode (otherwise the flags would not change)
            InfoManager.InfoMode oldMode = InfoManager.instance.CurrentMode;
            ReflectionHelper.SetAttr(InfoManager.instance, "m_actualMode", InfoManager.InfoMode.TrafficRoutes);
            InfoManager.SubInfoMode oldSubMode = InfoManager.instance.CurrentSubMode;
            ReflectionHelper.SetAttr(InfoManager.instance, "m_actualSubMode", InfoManager.SubInfoMode.WaterPower);

            // Apply node button click
            NetNode[] nodes = NetManager.instance.m_nodes.m_buffer;
            nodes[command.NodeId].Info.m_netAI.ClickNodeButton(command.NodeId, ref nodes[command.NodeId], command.Index);

            // Reset info manager mode
            ReflectionHelper.SetAttr(InfoManager.instance, "m_actualMode", oldMode);
            ReflectionHelper.SetAttr(InfoManager.instance, "m_actualSubMode", oldSubMode);

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
