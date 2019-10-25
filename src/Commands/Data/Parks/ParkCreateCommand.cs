using ProtoBuf;

namespace CSM.Commands.Data.Parks
{
    /// <summary>
    ///     Called when a park is created.
    /// </summary>
    /// Sent by:
    /// - DistrictHandler
    [ProtoContract]
    public class ParkCreateCommand : CommandBase
    {
        /// <summary>
        ///     The park id to create.
        /// </summary>
        [ProtoMember(1)]
        public byte ParkId { get; set; }

        /// <summary>
        ///     The type of the park to create.
        /// </summary>
        [ProtoMember(2)]
        public DistrictPark.ParkType ParkType { get; set; }

        /// <summary>
        ///     The level of the park to create.
        /// </summary>
        [ProtoMember(3)]
        public DistrictPark.ParkLevel ParkLevel { get; set; }
        
        /// <summary>
        ///     The random seed of the park (e.g. for the park name).
        /// </summary>
        [ProtoMember(4)]
        public ulong Seed { get; set; }
    }
}
