﻿using CSM.API.Commands;
using ProtoBuf;

namespace SampleExternalMod.Commands
{
    /// <summary>
    ///     This command is called when a player buys a new area.
    /// </summary>
    /// Sent by:
    /// - 
    [ProtoContract]
    public class TestCommand : CommandBase
    {
        [ProtoMember(1)] public string testing { get; set; }
    }
}
