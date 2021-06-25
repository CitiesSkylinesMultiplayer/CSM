using CSM.API.Commands;
using ICities;
using NLog;

namespace SampleExternalMod.Commands
{
    public class UnlockAreaHandler : CommandHandler<TestCommand>
    {
        private static readonly Logger _logger = LogManager.GetLogger("CSM");
        protected override void Handle(TestCommand command)
        {
            _logger.Info(command.testing);
            // DO SOMETHING HERE
        }
    }
}