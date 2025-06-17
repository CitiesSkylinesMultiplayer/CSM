using CSM.API.Commands;
using ProtoBuf;

namespace CSM.BaseGame.Commands.Data.Buildings
{
    /// <summary>
    ///     Called when the last index cache for randomizing the industry variant is updated.
    /// </summary>
    /// Sent by:
    /// - BuildingHandler
    [ProtoContract]
    public class BuildingUpdateIndustryLastIndexCommand : CommandBase
    {
        /// <summary>
        ///     The key of the changed index.
        /// </summary>
        [ProtoMember(1)]
        public uint Key { get; set; }

        /// <summary>
        ///     The new index value.
        /// </summary>
        [ProtoMember(2)]
        public int Value { get; set; }
    }
}
