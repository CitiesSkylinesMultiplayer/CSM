using ProtoBuf;

namespace CSM.Commands.Data.TransportLines
{
    /// <summary>
    ///     Called when a slider in the transport line info window was changed.
    ///     This covers the ticket price and vehicle count sliders.
    /// </summary>
    /// Sent by:
    /// - TransportLine
    [ProtoContract]
    public class TransportLineChangeSliderCommand : CommandBase
    {
        /// <summary>
        ///     The id of the line that was modified.
        /// </summary>
        [ProtoMember(1)]
        public ushort LineId { get; set; }

        /// <summary>
        ///     The new value of the slider.
        /// </summary>
        [ProtoMember(2)]
        public float Value { get; set; }

        /// <summary>
        ///     If true: Ticket price slider was changed
        ///     If false: Vehicle count slider was changed
        /// </summary>
        [ProtoMember(3)]
        public bool IsTicketPrice { get; set; }
    }
}
