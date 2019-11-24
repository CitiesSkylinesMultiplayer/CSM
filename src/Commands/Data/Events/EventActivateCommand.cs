using ProtoBuf;

namespace CSM.Commands.Data.Events
{
    /// <summary>
    ///     Called when an event is activated.
    /// </summary>
    /// Sent by:
    /// - EventHandler
    [ProtoContract]
    public class EventActivateCommand : CommandBase
    {
        /// <summary>
        ///     The id of the activated event.
        /// </summary>
        [ProtoMember(1)]
        public ushort Event { get; set; }
    }
}
