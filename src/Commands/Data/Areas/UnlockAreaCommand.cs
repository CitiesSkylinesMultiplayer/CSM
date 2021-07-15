using ProtoBuf;

namespace CSM.Commands.Data.Areas
{
    /// <summary>
    ///     This command is called when a player buys a new area.
    /// </summary>
    /// Sent by:
    /// - AreaExtension
    [ProtoContract]
    public class UnlockAreaCommand : CommandBase
    {
        [ProtoMember(1)]
        public int X { get; set; }

        [ProtoMember(2)]
        public int Z { get; set; }
    }
}
