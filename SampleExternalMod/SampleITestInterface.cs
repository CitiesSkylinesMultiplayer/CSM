using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSM.API;
using NLog;

namespace SampleExternalMod
{
    class SampleITestInterface : ITest
    {
        public Guid HandlerID => new Guid();

        public string Handle()
        {
            string test = "This is a sample Mod";
            Logger _logger = LogManager.GetLogger("CSM");
            _logger.Info(test);
            return test;
        }
    }
}
