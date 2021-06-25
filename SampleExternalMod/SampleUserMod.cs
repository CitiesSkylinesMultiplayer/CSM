using System;
using ICities;

namespace SampleExternalMod
{
    public class SampleUserMod : IUserMod
    {
        public String Name
        {
            get { return "Sample External Mod"; }
        }

        public String Description
        {
            get { return "Adds Nothing"; }
        }
    }
}
