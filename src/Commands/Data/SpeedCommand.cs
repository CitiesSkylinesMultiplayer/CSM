using ProtoBuf;

namespace CSM.Commands
{
    /// <summary>
    ///     The current game speed.
    /// </summary>
    [ProtoContract]
    public class SpeedCommand : CommandBase
    {
        [ProtoMember(1)]
        public int SelectedSimulationSpeed { get; set; }
    }
}
