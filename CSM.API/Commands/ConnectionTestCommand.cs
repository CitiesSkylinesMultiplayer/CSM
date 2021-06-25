using ProtoBuf;

namespace CSM.API.Commands
{
    class ConnectionTestCommand : CommandBase
    {
        /// <summary>
        ///     A string which is used to test the connection
        /// </summary>
        [ProtoMember(1)]
        public int connectionTestInt  { get; set; }
    }
}
