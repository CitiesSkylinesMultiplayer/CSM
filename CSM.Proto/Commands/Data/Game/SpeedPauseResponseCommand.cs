using ProtoBuf;

namespace CSM.Commands.Data.Game
{
    /// <summary>
    ///     Sent as a response to the SpeedPauseRequestCommand to share the current game time with all clients.
    /// </summary>
    /// Sent by:
    /// - SpeedPauseHelper
    [ProtoContract]
    public class SpeedPauseResponseCommand : CommandBase
    {
        /// <summary>
        ///     The current game time in DateTime ticks. 
        /// </summary>
        [ProtoMember(1)]
        public long CurrentTime { get; set; }

        /// <summary>
        ///     The id of the associated SpeedPauseRequest.
        /// </summary>
        [ProtoMember(2)]
        public int RequestId { get; set; }

        /// <summary>
        ///     The maximum latency of the connected clients in milliseconds.
        /// </summary>
        [ProtoMember(3)]
        public long MaxLatency { get; set; }
        
        /// <summary>
        ///     The number of connected clients.
        ///     Only filled by the server, otherwise -1.
        /// </summary>
        [ProtoMember(4)]
        public int NumberOfClients { get; set; }
    }
}
