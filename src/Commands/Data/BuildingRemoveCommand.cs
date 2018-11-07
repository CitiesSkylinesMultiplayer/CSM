using ProtoBuf;

namespace CSM.Commands
{
    /// <summary>
    ///     This command is sent when a building is removed.
    /// </summary>
    [ProtoContract]
    public class BuildingRemoveCommand : CommandBase
    {
        /// <summary>
        ///     The position of the building to be removed.
        /// </summary>
        [ProtoMember(1)]
        public uint BuildingID { get; set; }
    }
}