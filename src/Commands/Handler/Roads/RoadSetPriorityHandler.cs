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
            IgnoreHelper.EndIgnore();
        }
    }
}
