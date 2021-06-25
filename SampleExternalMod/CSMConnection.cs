using CSM.API;
using SampleExternalMod.Commands;

namespace SampleExternalMod
{
    class CSMConnection : Connection
    {

        public CSMConnection()
        {
            name = "Sample External Mod";
        }

        public void send()
        {
            TestCommand command = new TestCommand();
            command.testing = "This is a test mod";
            SentToAll(command);
        }
    }
}
