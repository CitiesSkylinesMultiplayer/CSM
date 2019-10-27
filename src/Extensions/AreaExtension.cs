using CSM.Commands;
using CSM.Commands.Data.Areas;
using CSM.Helpers;
using ICities;

namespace CSM.Extensions
{
    public class AreaExtension : AreasExtensionBase
    {
        public override void OnUnlockArea(int x, int z)
        {
            if (IgnoreHelper.IsIgnored())
                return;

            Command.SendToAll(new UnlockAreaCommand
            {
                X = x,
                Z = z
            });
        }
    }
}
