using ProtoBuf;

namespace CSM.Commands.Data.Buildings
{
    /// <summary>
    ///     Called when the transfer reason/material of building was changed.
    /// </summary>
    /// Sent by:
    /// - BuildingHandler
    [ProtoContract]
    public class BuildingSetTransferReasonCommand : CommandBase
    {
        /// <summary>
        ///     The id of the modified building.
        /// </summary>
        [ProtoMember(1)]
        public ushort Building { get; set; }

        /// <summary>
        ///     The new transfer reason/material.
        /// </summary>
        [ProtoMember(2)]
        public TransferManager.TransferReason Material { get; set; }
    }
}
