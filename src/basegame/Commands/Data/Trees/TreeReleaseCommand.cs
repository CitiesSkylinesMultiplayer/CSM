﻿using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Trees
{
    /// <summary>
    ///     Called when a tree is released.
    /// </summary>
    /// Sent by:
    /// - TreeHandler
    [ProtoContract]
    public class TreeReleaseCommand : CommandBase
    {
        /// <summary>
        ///     The id of the tree to release.
        /// </summary>
        [ProtoMember(1)]
        public uint TreeId { get; set; }
    }
}
