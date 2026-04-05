using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Events
{
    /// <summary>
    ///     Called when a race event is manually ended by a player.
    /// </summary>
    /// Sent by:
    /// - RaceEventHandler
    [ProtoContract]
    public class RaceEventEndCommand : CommandBase
    {
        /// <summary>
        ///     The id of the race event to end.
        /// </summary>
        [ProtoMember(1)]
        public ushort Event { get; set; }
    }
}
