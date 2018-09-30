using System.IO;
using ProtoBuf;

namespace CSM.Commands
{
    /// <summary>
    ///     Game simulation info
    /// </summary>
    [ProtoContract]
    public class PauseCommand : CommandBase
    {
        [ProtoMember(1)]
        public bool SimulationPaused { get; set; }

        /// <summary> 
        ///     Deserialize a message into this type.
        /// </summary>
        public static PauseCommand Deserialize(byte[] message)
        {
            PauseCommand result;

            using (var stream = new MemoryStream(message))
            {
                result = Serializer.Deserialize<PauseCommand>(stream);
            }

            return result;
        }
    }
}
