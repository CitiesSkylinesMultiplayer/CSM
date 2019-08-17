using ProtoBuf;
using System.Collections.Generic;

namespace CSM.Commands
{
    /// <summary>
    ///     This sends the current list of players.
    /// </summary>
    [ProtoContract]
    public class PlayerListCommand : CommandBase
    {
        [ProtoMember(1)]
        public HashSet<string> PlayerList { get; set; }
    }
}
