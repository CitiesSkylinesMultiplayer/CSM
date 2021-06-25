using System;
using CSM.API;

namespace SampleExternalMod
{
    class SampleITestInterface : ITest
    {
        private Func<string, byte[],bool> sendCommand;
        public Guid HandlerID => new Guid();

        public bool ConnectToCSM(Func<string, byte[], bool> function)
        {
            sendCommand = function;
            byte[] data = new byte[] { 0, 0, 1, 1 };
            transmitUpdate(data);
            return true;
        }

        public string Handle(byte[] data)
        {
            string test = "This is a sample Mod";
            return test;
        }

        public void transmitUpdate(byte[] someUpdate)
        {
            sendCommand("SamepleMod",someUpdate);
        }
    }
}
