using CSM.API.Commands;
using ProtoBuf;

namespace CSM.Commands.Data.Internal
{
    /// <summary>
    ///     Used to notify all clients that a new client is connecting (starting to and finished).
    /// </summary>
    /// Sent by:
    /// - ClientJoiningHandler
    /// - ClientLevelLoadedHandler
    /// - ConnectionRequestHandler
    [ProtoContract]
    public class ClientJoiningCommand : CommandBase
    {
        /// <summary>
        ///     False: A player starts to join (Game should be blocked)
        ///     True: A player finished joining (Game should be unblocked)
        /// </summary>
        [ProtoMember(1)]
        public bool JoiningFinished { get; set; }

        /// <summary>
        ///     Username of the joining player
        /// </summary>
        [ProtoMember(2)]
        public string JoiningUsername { get; set; }
    }
}
