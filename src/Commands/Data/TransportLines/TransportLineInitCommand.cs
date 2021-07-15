using ProtoBuf;

namespace CSM.Commands.Data.TransportLines
{
    /// <summary>
    ///     Called when the transport tool is initialized.
    /// </summary>
    /// Sent by:
    /// - TransportHandler
    [ProtoContract]
    public class TransportLineInitCommand : CommandBase
    {
    }
}
