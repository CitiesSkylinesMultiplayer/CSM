using CSM.API.Commands;
using ProtoBuf;

namespace CSM.Commands.Data.Economy
{
    /// <summary>
    ///     Sent when a player clicks one of the buttons in the bailout panel.
    /// </summary>
    /// Sent by:
    /// - EconomyHandler
    [ProtoContract]
    public class EconomyBailoutCommand : CommandBase
    {
        /// <summary>
        ///     If the bailout was accepted.
        /// </summary>
        [ProtoMember(1)]
        public bool Accepted { get; set; }
    }
}
