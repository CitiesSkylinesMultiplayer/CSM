using ProtoBuf;

namespace CSM.Commands.Data.Props
{
    /// <summary>
    ///     Called when a prop is released.
    /// </summary>
    /// Sent by:
    /// - PropHandler
    [ProtoContract]
    public class PropReleaseCommand : CommandBase
    {
        /// <summary>
        ///     The id of the prop to remove.
        /// </summary>
        [ProtoMember(1)]
        public ushort PropId { get; set; }
    }
}
