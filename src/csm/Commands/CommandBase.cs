using ProtoBuf;
using System.IO;

namespace CSM.Commands
{
    [ProtoContract]
    public abstract class CommandBase
    {
        #region Commands

        public const byte ConnectionRequestCommandId = 0;
        public const byte ConnectionResultCommandId = 1;
        public const byte PingCommandId = 3;

        public const byte SpeedCommandID = 100;
        public const byte PauseCommandID = 101;
        public const byte MoneyCommandID = 102;
        public const byte CreatedCommandID = 103;
        public const byte BuildingRemovedCommandID = 104;
        public const byte RoadCommandID = 110;

        #endregion Commands

        public byte[] Serialize()
        {
            byte[] result;

            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, this);
                result = stream.ToArray();
            }

            return result;
        }
    }
}