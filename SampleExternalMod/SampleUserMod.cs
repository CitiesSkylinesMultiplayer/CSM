using System;
using ICities;

namespace SampleExternalMod
{
    public class SampleUserMod : IUserMod
    {
        public string Name
        {
            get { return "Sample External Mod"; }
        }

        public string Description
        {
            get { return "Adds Nothing"; }
        }
    }
}
