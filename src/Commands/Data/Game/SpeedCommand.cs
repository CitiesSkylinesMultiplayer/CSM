using ProtoBuf;

namespace CSM.Commands.Data.Game
{
    /// <summary>
    ///     Sent when the game speed is changed.
    /// </summary>
    /// Sent by:
    /// - Threading extension
    [ProtoContract]
    public class SpeedCommand : CommandBase
    {
        /// <summary>
        ///     The current game speed.
        /// </summary>
        [ProtoMember(1)]
        public int SelectedSimulationSpeed { get; set; }
    }
}
