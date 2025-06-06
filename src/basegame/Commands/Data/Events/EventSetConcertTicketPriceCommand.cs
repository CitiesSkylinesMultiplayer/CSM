using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Events
{
    /// <summary>
    ///     Called when the ticket price of a concert was changed.
    /// </summary>
    /// Sent by:
    /// - EventHandler
    [ProtoContract]
    public class EventSetConcertTicketPriceCommand : CommandBase
    {
        /// <summary>
        ///     The building id of the related panel.
        /// </summary>
        [ProtoMember(1)]
        public ushort Building { get; set; }
        /// <summary>
        ///     The event info id of the concert that was modified.
        /// </summary>
        [ProtoMember(2)]
        public uint Event { get; set; }

        /// <summary>
        ///     The new ticket price.
        /// </summary>
        [ProtoMember(3)]
        public int Price { get; set; }
    }
}
