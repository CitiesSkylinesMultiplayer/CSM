using System;
using System.IO;
using ProtoBuf;

namespace CitiesSkylinesMultiplayer.Commands
{
    /// <summary>
    ///     Ping command
    /// </summary>
    [ProtoContract]
    public class Ping : CommandBase
    {
        /// <summary> 
        ///     Deserialize a message into this type.
        /// </summary>
        public static Ping Deserialize(byte[] message)
        {
            Ping result;

            using (var stream = new MemoryStream(message))
            {
                result = Serializer.Deserialize<Ping>(stream);
            }

            return result;
        }
    }
}
