using ProtoBuf;

namespace CSM.Commands
{
    /// <summary>
    ///     Send chat messages to other players in game
    /// </summary>
    [ProtoContract]
    public class ChatMessageCommand : CommandBase
    {
        /// <summary>
        ///     The username of the person who sent the message.
        /// </summary>
        [ProtoMember(1)]
        public string Username { get; set; }

        /// <summary>
        ///     The message that the user sent.
        /// </summary>
        [ProtoMember(2)]
        public string Message { get; set; }
    }
}