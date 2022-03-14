using CSM.API.Commands;
using CSM.API.Helpers;
using CSM.BaseGame.Commands.Data.Terrain;

namespace CSM.BaseGame.Commands.Handler.Terrain
{
    public class SoilTradeHandler : CommandHandler<SoilTradeCommand>
    {
        protected override void Handle(SoilTradeCommand command)
        {
            IgnoreHelper.Instance.StartIgnore();

            TerrainManager.instance.DirtBuffer = command.DirtBuffer;

            IgnoreHelper.Instance.EndIgnore();
        }
    }
}
