using ProtoBuf;

namespace CSM.Commands.Data.Game
{
    /// <summary>
    ///     Sent when the game speed or pause state is changed to request a SpeedPauseResponseCommand.
    /// </summary>
    /// Sent by:
    /// - SpeedPauseHelper
    [ProtoContract]
    [FixedCommand(90017)]
    public class SpeedPauseRequestCommand : CommandBase
    {
        /// <summary>
        ///     The current game speed.
        ///     Not set when SimulationPaused equals true.
        /// </summary>
        [ProtoMember(1)]
        public int SelectedSimulationSpeed { get; set; }
        
        /// <summary>
        ///     If the simulation is paused.
        /// </summary>
        [ProtoMember(2)]
        public bool SimulationPaused { get; set; }

        /// <summary>
        ///     The request id to uniquely identify this SpeedPauseRequest.
        ///     This id will be used in the SpeedPauseResponse.
        ///     Not set if this command is used to request the play state.
        /// </summary>
        [ProtoMember(3)]
        public int RequestId { get; set; }
    }
}
