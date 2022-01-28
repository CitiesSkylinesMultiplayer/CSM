using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Events
{
    /// <summary>
    ///     Called when the ticket price of an event was changed.
    /// </summary>
    /// Sent by:
    /// - EventHandler
    [ProtoContract]
    public class EventSetTicketPriceCommand : CommandBase
    {
        /// <summary>
        ///     The id of the event that was modified.
        /// </summary>
        [ProtoMember(1)]
        public ushort Event { get; set; }

        /// <summary>
        ///     The new ticket price.
        /// </summary>
        [ProtoMember(2)]
        public int Price { get; set; }
    }
}
