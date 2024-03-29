using CSM.API.Commands;
using ProtoBuf;

namespace CSM.Commands.Data.Game
{
    /// <summary>
    ///     Sent when the requested new state was reached.
    /// </summary>
    /// Sent by:
    /// - SpeedPauseHelper
    [ProtoContract]
    public class SpeedPauseReachedCommand : CommandBase
    {
        /// <summary>
        ///     The id of the associated SpeedPauseRequest.
        /// </summary>
        [ProtoMember(1)]
        public int RequestId { get; set; }
    }
}
