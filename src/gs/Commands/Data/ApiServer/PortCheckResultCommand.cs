using ProtoBuf;

namespace CSM.GS.Commands.Data.ApiServer
{
    /// <summary>
    ///     Response of the global server to check the port
    /// </summary>
    /// Sent by:
    /// - Global Server
    [ProtoContract]
    public class PortCheckResultCommand : ApiCommandBase
    {
        /// <summary>
        ///     The determined state of the checked port.
        /// </summary>
        [ProtoMember(1)]
        public PortCheckResult State { get; set; }

        /// <summary>
        ///     The error message in case the port check failed.
        /// </summary>
        [ProtoMember(2)]
        public string Message { get; set; }
    }

    public enum PortCheckResult
    {
        Reachable,
        Unreachable,
        Error
    }
}
