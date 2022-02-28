using ICities;

namespace SampleExternalMod
{
    public class SampleUserMod : IUserMod
    {
        public string Name => "Sample External Mod";

        public string Description => "Adds Nothing";

        public void OnEnabled()
        {
            // Normal mod stuff here
        }

        public void OnDisabled()
        {
            // Normal mod stuff here
        }
    }
}
