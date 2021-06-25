using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSM.Commands.Data.API
{
    /// <summary>
    ///     Called when a mod sends a command.
    /// </summary>
    /// Sent by:
    /// - ModSupport
    class ExternalAPICommand : CommandBase
    {
        /// <summary>
        ///     The name of the mod.
        /// </summary>
        [ProtoMember(1)]
        public string Name { get; set; }

        /// <summary>
        ///     The data to be sent
        /// </summary>
        [ProtoMember(2)]
        public byte[] Data { get; set; }
    }
}
