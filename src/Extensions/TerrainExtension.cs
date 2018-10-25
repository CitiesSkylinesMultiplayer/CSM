using ICities;

namespace CSM.Extensions
{
    public class TerrainExtension : TerrainExtensionBase
    {
        public override void OnAfterHeightsModified(float minX, float minZ, float maxX, float maxZ)
        {
            base.OnAfterHeightsModified(minX, minZ, maxX, maxZ);
        }
    }
}