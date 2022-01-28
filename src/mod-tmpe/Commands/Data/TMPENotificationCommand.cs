using CSM.API.Commands;
using ProtoBuf;

namespace CSM.TMPE.Commands.Data
{
    /// <summary>
    ///     This command is used to send notifications of TMPE changes.
    /// </summary>
    [ProtoContract]
    public class TMPENotification : CommandBase
    {
        /// <summary>
        /// Gets or sets the record object with base64 encoding.
        /// </summary>
        [ProtoMember(1)]
        public string Base64RecordObject { get; set; }

        /// <summary>
        /// Gets or sets the data version.
        /// </summary>
        [ProtoMember(2)]
        public string DataVersion { get; set; }
    }
}
