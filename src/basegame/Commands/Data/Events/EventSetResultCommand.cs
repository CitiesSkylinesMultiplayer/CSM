using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Events
{
    /// <summary>
    ///     Called when the the result of an event (sports match) is set.
    /// </summary>
    /// Sent by:
    /// - EventHandler
    [ProtoContract]
    public class EventSetResultCommand : CommandBase
    {
        /// <summary>
        ///     The id of the event that was modified.
        /// </summary>
        [ProtoMember(1)]
        public ushort Event { get; set; }

        /// <summary>
        ///     The event result
        /// </summary>
        [ProtoMember(2)]
        public bool Result { get; set; }
    }
}
