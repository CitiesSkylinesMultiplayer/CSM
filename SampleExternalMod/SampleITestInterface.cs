using System;
using CSM.API;
using CSM.API.Commands;
using SampleExternalMod.Commands;

namespace SampleExternalMod
{
    class SampleITestInterface : ITest
    {
        private Func<CommandBase, bool> sendCommand;
        public Guid HandlerID => new Guid();

        public string name => "Sample Mod";

        public bool ConnectToCSM(Func<CommandBase, bool> function)
        {
            sendCommand = function;
            TestCommand testCommand = new TestCommand();
            testCommand.testing = "this is a test" ;
            transmitCommand(testCommand);
            return true;
        }

        public string Handle(byte[] data)
        {
            string test = "This is a sample Mod";
            return test;
        }

        public void transmitCommand(CommandBase testCommand)
        {
            sendCommand(testCommand);
        }
    }
}
