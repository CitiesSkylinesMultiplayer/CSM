using ProtoBuf;

namespace CSM.Commands
{
    /// <summary>
    ///     Ping command used to tell the server that a client
    ///     is still connected.
    /// </summary>
    [ProtoContract]
    public class PingCommand : CommandBase
    { }
}