﻿using ProtoBuf;

namespace CSM.Commands.Data.Internal
{
    /// <summary>
    ///     This commands transfers the save game.
    /// </summary>
    /// Sent by:
    /// - ConnectionRequestHandler
    [ProtoContract]
    public class WorldTransferCommand : CommandBase
    {
        /// <summary>
        ///     The serialized save game.
        /// </summary>
        [ProtoMember(1)]
        public byte[] World { get; set; }
    }
}
