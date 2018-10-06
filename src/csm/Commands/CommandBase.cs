using System.CodeDom;
using System.IO;
using ProtoBuf;

namespace CSM.Commands
{
    [ProtoContract]
    public abstract class CommandBase
    {
        public const byte ConnectionRequestCommandId = 0;
        public const byte ConnectionResultCommandId = 1;
        public const byte PingCommandId = 3;

        public const byte SpeedCommandID = 100;
		public const byte PauseCommandID = 101;
		public const byte MoneyCommandID = 102;


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