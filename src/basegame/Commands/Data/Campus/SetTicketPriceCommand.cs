using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Campus
{
    /// <summary>
    ///     Called when the ticket price for varsity sports is set on a campus.
    /// </summary>
    /// Sent by:
    /// - CampusHandler
    [ProtoContract]
    public class SetTicketPriceCommand : CommandBase
    {
        /// <summary>
        ///     The park id of the campus.
        /// </summary>
        [ProtoMember(1)]
        public byte Campus { get; set; }

        /// <summary>
        ///     The new ticket price.
        /// </summary>
        [ProtoMember(2)]
        public ushort Price { get; set; }
    }
}
