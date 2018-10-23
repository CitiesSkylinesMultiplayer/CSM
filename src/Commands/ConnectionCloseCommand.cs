using ProtoBuf;

namespace CSM.Commands
{
    /// <summary>
    ///     The message the client sends before it closes the connection.
    /// </summary>
    [ProtoContract]
    public class ConnectionCloseCommand : CommandBase
    { }
}