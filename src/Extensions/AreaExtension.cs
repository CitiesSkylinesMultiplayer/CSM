using CSM.Commands;
using ICities;

namespace CSM.Extensions
{
    public class AreaExtension : AreasExtensionBase
    {
        public override void OnUnlockArea(int x, int z)
        {
            Command.SendToAll(new UnlockAreaCommand
            {
                X = x,
                Z = z
            });
        }
    }
}
