using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Areas;
using ICities;

namespace CSM.BaseGame.Extensions
{
    public class AreaExtension : AreasExtensionBase
    {
        public override void OnUnlockArea(int x, int z)
        {
            if (IgnoreHelper.Instance.IsIgnored())
                return;

            Command.SendToAll(new UnlockAreaCommand
            {
                X = x,
                Z = z
            });
        }
    }
}
