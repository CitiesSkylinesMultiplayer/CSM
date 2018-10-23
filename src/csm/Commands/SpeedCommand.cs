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
    }
}