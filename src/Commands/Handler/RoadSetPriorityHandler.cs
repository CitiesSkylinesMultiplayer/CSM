using CSM.Injections;

namespace CSM.Commands.Handler
{
    public class RoadSetPriorityHandler : CommandHandler<RoadSetPriorityCommand>
    {
        public override void Handle(RoadSetPriorityCommand command)
        {
            RoadHandler.IgnoreAll = true;
            NetManager.instance.SetPriorityRoad(command.SegmentId, command.Priority).MoveNext();
            RoadHandler.IgnoreAll = false;
        }
    }
}
