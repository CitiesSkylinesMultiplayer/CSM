using ProtoBuf;

namespace CSM.Commands.Data.Internal
{
    /// <summary>
    ///     Sends the number of dropped frames to the other clients (every two seconds).
    /// </summary>
    /// Sent by:
    /// - TickLoopHandler
    [ProtoContract]
    public class SlowdownCommand : CommandBase
    {
        /// <summary>
        ///     The number of dropped frames.
        /// </summary>
        [ProtoMember(1)]
        public int DroppedFrames { get; set; }
    }
}
