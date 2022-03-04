using CSM.API;
using ICities;

namespace CSM.BaseGame
{
    public class BaseGameConnection : Connection
    {
        public BaseGameConnection()
        {
            Name = "Cities: Skylines";
            Enabled = true;
            CommandAssemblies.Add(typeof(BaseGameConnection).Assembly);
            ModClass = null; // Explicitly null in this case (this is not a mod)
        }

        public override void RegisterHandlers()
        {
            Patcher.PatchAll();
        }

        public override void UnregisterHandlers()
        {
            Patcher.UnpatchAll();
        }
    }
}
