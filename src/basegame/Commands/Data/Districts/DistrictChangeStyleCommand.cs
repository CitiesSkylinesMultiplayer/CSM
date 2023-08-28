using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Districts
{
    /// <summary>
    ///     Called when the building style of a district is changed.
    /// </summary>
    [ProtoContract]
    public class DistrictChangeStyleCommand : CommandBase
    {
        /// <summary>
        ///     The district style to set.
        /// </summary>
        [ProtoMember(1)]
        public ushort Style { get; set; }

        /// <summary>
        ///     The modified district.
        /// </summary>
        [ProtoMember(2)]
        public byte DistrictId { get; set; }
    }
}
