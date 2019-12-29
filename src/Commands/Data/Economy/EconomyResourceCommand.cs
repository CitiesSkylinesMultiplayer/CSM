using ProtoBuf;

namespace CSM.Commands.Data.Economy
{
    /// <summary>
    ///     This sends changes of resources (Add and fetch).
    /// </summary>
    /// Sent by:
    /// - EconomyHandler
    [ProtoContract]
    public class EconomyResourceCommand : CommandBase
    {
        /// <summary>
        ///     The modified amount.
        /// </summary>
        [ProtoMember(1)]
        public int ResourceAmount { get; set; }

        /// <summary>
        ///     The action that was performed (ADD, FETCH, PRIVATE)
        /// </summary>
        [ProtoMember(2)]
        public ResourceAction Action { get; set; }

        /// <summary>
        ///     The modified resource type.
        /// </summary>
        [ProtoMember(3)]
        public EconomyManager.Resource ResourceType { get; set; }

        /// <summary>
        ///     The corresponding service.
        /// </summary>
        [ProtoMember(4)]
        public ItemClass.Service Service { get; set; }

        /// <summary>
        ///     The corresponding subservice.
        /// </summary>
        [ProtoMember(5)]
        public ItemClass.SubService SubService { get; set; }

        /// <summary>
        ///     The corresponding level.
        /// </summary>
        [ProtoMember(6)]
        public ItemClass.Level Level { get; set; }

        /// <summary>
        ///     The corresponding taxation policies for added resources.
        /// </summary>
        [ProtoMember(7)]
        public DistrictPolicies.Taxation Taxation { get; set; }

        /// <summary>
        ///     The corresponding tax rate for adding private income.
        /// </summary>
        [ProtoMember(8)]
        public int TaxRate { get; set; }
    }

    public enum ResourceAction
    {
        ADD,
        FETCH,
        PRIVATE
    }
}
