using ProtoBuf;

namespace CSM.Commands.Data.Game
{
    /// <summary>
    ///     Sent when the user toggles between a paused simulation and
    ///     playing simulation.
    /// </summary>
    /// Sent by:
    /// - ThreadingExtension
    [ProtoContract]
    public class PauseCommand : CommandBase
    {
        /// <summary>
        ///     If the simulation is paused.
        /// </summary>
        [ProtoMember(1)]
        public bool SimulationPaused { get; set; }
    }
}
