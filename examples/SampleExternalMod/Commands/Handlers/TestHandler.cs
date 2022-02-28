using CSM.API.Commands;
using SampleExternalMod.Commands.Data;

namespace SampleExternalMod.Commands.Handlers
{
    public class TestHandler : CommandHandler<TestCommand>
    {
        protected override void Handle(TestCommand command)
        {
            // Handle command
        }
    }
}
