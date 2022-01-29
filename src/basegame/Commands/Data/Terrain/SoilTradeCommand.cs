using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Terrain
{
    /// <summary>
    ///     Called when soil is bought or sold
    /// </summary>
    /// Sent by:
    /// - TerrainHandler
    [ProtoContract]
    public class SoilTradeCommand : CommandBase
    {
        /// <summary>
        ///     The new value of the dirt buffer.
        /// </summary>
        [ProtoMember(1)]
        public int DirtBuffer { get; set; }
    }
}
