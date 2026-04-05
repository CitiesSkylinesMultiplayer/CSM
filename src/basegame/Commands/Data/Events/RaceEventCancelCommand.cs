using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Events
{
    /// <summary>
    ///     Called when a race event is cancelled by a player.
    /// </summary>
    /// Sent by:
    /// - RaceEventHandler
    [ProtoContract]
    public class RaceEventCancelCommand : CommandBase
    {
        /// <summary>
        ///     The id of the race event to cancel.
        /// </summary>
        [ProtoMember(1)]
        public ushort Event { get; set; }
    }
}
