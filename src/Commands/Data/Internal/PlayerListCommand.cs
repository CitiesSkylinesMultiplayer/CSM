﻿using ProtoBuf;
using System.Collections.Generic;

namespace CSM.Commands.Data.Internal
{
    /// <summary>
    ///     This sends the current list of players.
    /// </summary>
    [ProtoContract]
    public class PlayerListCommand : CommandBase
    {
        /// <summary>
        ///     The list of player names.
        /// </summary>
        [ProtoMember(1)]
        public HashSet<string> PlayerList { get; set; }
    }
}
