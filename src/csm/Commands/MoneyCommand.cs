using ProtoBuf;
using System.IO;

namespace CSM.Commands
{
    /// <summary>
    ///     This sends the current cash amount
    ///    TODO: find out how top copy an array to reflection
    /// /// </summary>
    [ProtoContract]
    public class MoneyCommand : CommandBase
    {
        [ProtoMember(1)]
        public long InternalMoneyAmount { get; set; }

        //[ProtoMember(2)]
        //public long[] TotalIncome { get; set; }

        //[ProtoMember(3)]
        //public long[] TotalExpenses { get; set; }

        /// <summary>
        ///     Deserialize a message into this type.
        /// </summary>
        public static MoneyCommand Deserialize(byte[] message)
        {
            MoneyCommand result;

            using (var stream = new MemoryStream(message))
            {
                result = Serializer.Deserialize<MoneyCommand>(stream);
            }

            return result;
        }
    }
}