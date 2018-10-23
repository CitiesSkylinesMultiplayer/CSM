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
    }
}