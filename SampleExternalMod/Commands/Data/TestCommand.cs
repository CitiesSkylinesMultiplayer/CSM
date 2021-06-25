using ProtoBuf;

namespace CSM.API.Commands.Data
{
    /// <summary>
    ///     This command is called when a player buys a new area.
    /// </summary>
    /// Sent by:
    /// - AreaExtension
    [ProtoContract]
    public class TestCommand : CommandBase
    {
        [ProtoMember(1)] public int X { get; set; }

        [ProtoMember(2)] public int Z { get; set; }
    }
}