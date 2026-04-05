using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Events
{
    /// <summary>
    ///     Called when a race event is manually started by a player.
    /// </summary>
    /// Sent by:
    /// - RaceEventHandler
    [ProtoContract]
    public class RaceEventStartCommand : CommandBase
    {
        /// <summary>
        ///     The id of the race event to start.
        /// </summary>
        [ProtoMember(1)]
        public ushort Event { get; set; }
    }
}
