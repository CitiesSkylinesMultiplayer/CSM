using System.IO;
using ProtoBuf;

namespace CSM.Commands
{
    /// <summary>
    ///     Game simulation info
    /// </summary>
    [ProtoContract]
    public class SimulationCommand : CommandBase
    {
        [ProtoMember(1)]
        public int SelectedSimulationSpeed { get; set; }

        [ProtoMember(2)]
        public bool SimulationPaused { get; set; }

        [ProtoMember(3)]
        public bool ForcedSimulationPaused { get; set; }

        /// <summary> 
        ///     Deserialize a message into this type.
        /// </summary>
        public static SimulationCommand Deserialize(byte[] message)
        {
            SimulationCommand result;

            using (var stream = new MemoryStream(message))
            {
                result = Serializer.Deserialize<SimulationCommand>(stream);
            }

            return result;
        }
    }
}
