using ProtoBuf;

namespace CSM.Commands
{
    /// <summary>
    ///     Sent when the user toggles between a paused simulation and
    ///     playing simulation.
    /// </summary>
    [ProtoContract]
    public class PauseCommand : CommandBase
    {
        [ProtoMember(1)]
        public bool SimulationPaused { get; set; }
    }
}