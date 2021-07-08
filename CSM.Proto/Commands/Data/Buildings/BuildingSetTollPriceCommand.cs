using ProtoBuf;

namespace CSM.Commands.Data.Buildings
{
    /// <summary>
    ///     Called when the price of a toll station was changed.
    /// </summary>
    /// Sent by:
    /// - BuildingHandler
    [ProtoContract]
    public class BuildingSetTollPriceCommand : CommandBase
    {
        /// <summary>
        ///     The id of the modified building.
        /// </summary>
        [ProtoMember(1)]
        public ushort Building { get; set; }

        /// <summary>
        ///     The new price setting.
        /// </summary>
        [ProtoMember(2)]
        public int Price { get; set; }
    }
}
