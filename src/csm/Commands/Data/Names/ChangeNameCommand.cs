using CSM.API.Commands;
using ProtoBuf;

namespace CSM.Commands.Data.Names
{
    /// <summary>
    ///     Called when a name of following game features was changed:
    ///     Building, Citizen, CitizenInstance, Disaster, District, Park,
    ///     Event, NetSegment, TransportLine, Vehicle, ParkedVehicle
    /// </summary>
    /// Sent by:
    /// - NameHandler
    [ProtoContract]
    public class ChangeNameCommand : CommandBase
    {
        /// <summary>
        ///     The type of the changed element.
        /// </summary>
        [ProtoMember(1)]
        public InstanceType Type { get; set; }

        /// <summary>
        ///     The id of the changed element as an int.
        ///     Can be later casted into a uint, short or ushort.
        /// </summary>
        [ProtoMember(2)]
        public int Id { get; set; }

        /// <summary>
        ///     The new name of the game element.
        /// </summary>
        [ProtoMember(3)]
        public string Name { get; set; }
    }
}
