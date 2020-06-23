using ColossalFramework.UI;
using CSM.Commands.Data.Roads;
using CSM.Helpers;

namespace CSM.Commands.Handler.Roads
{
    public class RoadSetPriorityHandler : CommandHandler<RoadSetPriorityCommand>
    {
        protected override void Handle(RoadSetPriorityCommand command)
        {
            IgnoreHelper.StartIgnore();
            NetManager.instance.SetPriorityRoad(command.SegmentId, command.Priority).MoveNext();

            InstanceID instance = InfoPanelHelper.GetInstanceID(typeof(RoadWorldInfoPanel), out WorldInfoPanel panel);
            if (instance.Type == InstanceType.NetSegment && instance.NetSegment == command.SegmentId)
            {
                UICheckBox isPriority = ReflectionHelper.GetAttr<UICheckBox>(panel, "m_PriorityRoad");
                SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
                {
                    isPriority.isChecked = command.Priority;
                });
            }

            IgnoreHelper.EndIgnore();
        }
    }
}
