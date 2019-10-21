using CSM.Common;
using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class RoadSettingsHandler : CommandHandler<RoadSettingsCommand>
    {
        public override void Handle(RoadSettingsCommand command)
        {
            RoadHandler.IgnoreAll = true;
            
            // Simulate traffic routes mode (otherwise the flags would not change)
            InfoManager.InfoMode oldMode = InfoManager.instance.CurrentMode; 
            ReflectionHelper.SetAttr(InfoManager.instance, "m_actualMode", InfoManager.InfoMode.TrafficRoutes);
            InfoManager.SubInfoMode oldSubMode = InfoManager.instance.CurrentSubMode;
            ReflectionHelper.SetAttr(InfoManager.instance, "m_actualSubMode", InfoManager.SubInfoMode.WaterPower);

            // Apply node button click
            NetNode[] nodes= NetManager.instance.m_nodes.m_buffer;
            nodes[command.NodeId].Info.m_netAI.ClickNodeButton(command.NodeId, ref nodes[command.NodeId], command.Index);
            
            // Reset info manager mode
            ReflectionHelper.SetAttr(InfoManager.instance, "m_actualMode", oldMode);
            ReflectionHelper.SetAttr(InfoManager.instance, "m_actualSubMode", oldSubMode);

            RoadHandler.IgnoreAll = false;
        }
    }
}
