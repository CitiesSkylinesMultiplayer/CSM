using System;
using CSM.API.Commands;
using ProtoBuf;
using UnityEngine;

namespace CSM.BaseGame.Injections.Tools {

    [ProtoContract]
    public abstract class ToolCommandBase: CommandBase, IEquatable<ToolCommandBase> {

        [ProtoMember(1)]
        public string PlayerName { get; set; }
        [ProtoMember(2)]
        public Vector3 CursorWorldPosition { get; set; }

        public bool Equals(ToolCommandBase other)
        {
            return other != null &&
                PlayerName == other.PlayerName &&
                Equals(CursorWorldPosition, other.CursorWorldPosition);
        }
    }
}
