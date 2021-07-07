using ProtoBuf;

namespace CSM.Commands.Data.Internal
{
    /// <summary>
    ///     Used to notify the server that the world on the client has finished loading.
    /// </summary>
    /// Sent by:
    /// - LoadingExtension
    [ProtoContract]
    public class ClientLevelLoadedCommand : CommandBase
    {
    }
}
