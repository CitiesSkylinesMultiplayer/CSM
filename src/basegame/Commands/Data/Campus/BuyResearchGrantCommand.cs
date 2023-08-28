using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Campus
{
    /// <summary>
    ///     Called when a new research grant is bought for a campus.
    /// </summary>
    /// Sent by:
    /// - CampusHandler
    [ProtoContract]
    public class BuyResearchGrantCommand : CommandBase
    {
        /// <summary>
        ///     The park id of the campus.
        /// </summary>
        [ProtoMember(1)]
        public byte Campus { get; set; }

        /// <summary>
        ///     The type of the research grant.
        /// </summary>
        [ProtoMember(2)]
        public byte GrantType { get; set; }
    }
}
