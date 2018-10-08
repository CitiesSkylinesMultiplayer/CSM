using ProtoBuf;
using System.IO;

namespace CSM.Commands
{
    /// <summary>
    ///     Ping command
    /// </summary>
    [ProtoContract]
    public class PingCommand : CommandBase
    {
        /// <summary>
        ///     Deserialize a message into this type.
        /// </summary>
        public static PingCommand Deserialize(byte[] message)
        {
            PingCommand result;

            using (var stream = new MemoryStream(message))
            {
                result = Serializer.Deserialize<PingCommand>(stream);
            }

            return result;
        }
    }
}