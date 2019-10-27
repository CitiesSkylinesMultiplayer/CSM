using ProtoBuf;

namespace CSM.Commands.Data.Names
{
    /// <summary>
    ///     Called when the city name is changed.
    /// </summary>
    /// Sent by:
    /// - NameHandler
    [ProtoContract]
    public class ChangeCityNameCommand : CommandBase
    {
        /// <summary>
        ///     The new city name.
        /// </summary>
        [ProtoMember(1)]
        public string Name { get; set; }
    }
}
