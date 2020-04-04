using ProtoBuf;

namespace CSM.Commands.Data.Buildings
{
    /// <summary>
    ///     Called when any building needs to update its flags (Construction Status, Needs Status)
    /// </summary>
    /// Sent by:
    /// - BuildingHandler
    [ProtoContract]
    public class BuildingUpdateFlagsCommand : CommandBase
    {
        /// <summary>
        ///     The id of the building that was rebuilt.
        /// </summary>
        [ProtoMember(1)]
        public ushort Building { get; set; }

        /// <summary>
        ///     The Mask of Flags that is passed to UpdateFlags function
        /// </summary>
        [ProtoMember(2)]
        public Building.Flags ChangeMask { get; set; }
    }
}
