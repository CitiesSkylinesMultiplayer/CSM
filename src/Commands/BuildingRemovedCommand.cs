using ProtoBuf;
using UnityEngine;

namespace CSM.Commands
{
    /// <summary>
    ///     This command is sent when a building is removed.
    /// </summary>
    [ProtoContract]
    public class BuildingRemovedCommand : CommandBase
    {
        /// <summary>
        ///     The position of the building to be removed.
        /// </summary>
        [ProtoMember(1)]
        public Vector3 Position { get; set; }
    }
}