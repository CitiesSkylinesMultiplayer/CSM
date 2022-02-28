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
        }

        public override void RegisterHandlers(LoadMode mode)
        {
            Patcher.PatchAll();
        }

        public override void UnregisterHandlers()
        {
            Patcher.UnpatchAll();
        }
    }
}
