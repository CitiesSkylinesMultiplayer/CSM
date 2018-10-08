using System.IO;
using ProtoBuf;

namespace CSM.Commands
{
    /// <summary>
    ///     Game simulation info
    /// </summary>
    [ProtoContract]
    public class SpeedCommand : CommandBase
    {
        [ProtoMember(1)]
        public int SelectedSimulationSpeed { get; set; }

        /// <summary> 
        ///     Deserialize a message into this type.
        /// </summary>
        public static SpeedCommand Deserialize(byte[] message)
        {
            SpeedCommand result;

            using (var stream = new MemoryStream(message))
            {
                result = Serializer.Deserialize<SpeedCommand>(stream);
            }

            return result;
        }
    }
}
