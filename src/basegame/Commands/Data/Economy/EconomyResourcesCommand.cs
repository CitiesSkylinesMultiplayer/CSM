using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Economy
{
    /// <summary>
    ///     Sums up multiple EconomyResourceCommands for performance reasons.
    /// </summary>
    /// Sent by:
    /// - EconomyHandler
    [ProtoContract]
    public class EconomyResourcesCommand : CommandBase
    {
        /// <summary>
        ///     The list of EconomyResourceCommands.
        /// </summary>
        [ProtoMember(1)]
        public EconomyResourceCommand[] Commands;
    }
}
