using CSM.API.Commands;
using ProtoBuf;

namespace CSM.Commands.Data.Buildings
{
    /// <summary>
    ///     Called when a building is emptied or filled.
    ///     This includes emptying a landfill side or cemetery
    ///     and switching the 'allow intercity trains' flag on a train station.
    /// </summary>
    [ProtoContract]
    public class BuildingSetEmptyingFillingCommand : CommandBase
    {
        /// <summary>
        ///     The id of the modified building.
        /// </summary>
        [ProtoMember(1)]
        public ushort Building { get; set; }

        /// <summary>
        ///     The new value of the setting.
        /// </summary>
        [ProtoMember(2)]
        public bool Value { get; set; }

        /// <summary>
        ///     True if SetEmptying was called, false when SetFilling was called.
        /// </summary>
        [ProtoMember(3)]
        public bool SetEmptying { get; set; }
    }
}
