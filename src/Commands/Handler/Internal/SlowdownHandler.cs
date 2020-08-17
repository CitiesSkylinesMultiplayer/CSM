using CSM.Commands.Data.Internal;
using CSM.Helpers;

namespace CSM.Commands.Handler.Internal
{
    public class SlowdownHandler : CommandHandler<SlowdownCommand>
    {
        public SlowdownHandler()
        {
            TransactionCmd = false;
        }

        protected override void Handle(SlowdownCommand command)
        {
            SlowdownHelper.AddDropFrames(command.DroppedFrames);
        }
    }
}
