using ProtoBuf;

namespace CSM.Commands.Data.Events
{
    /// <summary>
    ///     Called when the security budget of an event was changed.
    /// </summary>
    /// Sent by:
    /// - EventHandler
    [ProtoContract]
    public class EventSetSecurityBudgetCommand : CommandBase
    {
        /// <summary>
        ///     The id of the modified event.
        /// </summary>
        [ProtoMember(1)]
        public ushort Event { get; set; }

        /// <summary>
        ///     The new security budget.
        /// </summary>
        [ProtoMember(2)]
        public int Budget { get; set; }
    }
}
