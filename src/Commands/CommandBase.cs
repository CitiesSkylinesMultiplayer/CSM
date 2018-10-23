using ProtoBuf;
using System.IO;

namespace CSM.Commands
{
    /// <summary>
    ///     A base protobuf command that all other commands in this mod should
    ///     extend. Provides support for serialization and deserialization.
    ///
    ///     When creating new commands, you should create a new command ID (up to 255) which
    ///     represents this command when sending over the network.
    /// </summary>
    [ProtoContract]
    public abstract class CommandBase
    {
        #region Commands

        public const byte ConnectionRequestCommandId = 0;
        public const byte ConnectionResultCommandId = 1;
        public const byte ConnectionCloseCommandId = 2;
        public const byte PingCommandId = 3;

        public const byte ClientConnectCommandId = 50;
        public const byte ClientDisconnectCommandId = 51;
        public const byte PlayerListCommand = 52;

        public const byte SpeedCommandID = 100;
        public const byte PauseCommandID = 101;
        public const byte MoneyCommandID = 102;
        public const byte BuildingCreatedCommandID = 103;
        public const byte BuildingRemovedCommandID = 104;
        public const byte RoadCommandID = 110;

        #endregion Commands

        /// <summary>
        ///     Serializes the command into a byte array for sending over the network.
        /// </summary>
        /// <returns>A byte array containing the message.</returns>
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

        /// <summary>
        ///     Deserializes a commmand from a byte array.
        /// </summary>
        /// <typeparam name="T">The type of message to deserialize to.</typeparam>
        /// <param name="message">A byte array of the message</param>
        /// <returns>The deserialized command.</returns>
        public static T Deserialize<T>(byte[] message) where T : CommandBase
        {
            T result;

            using (var stream = new MemoryStream(message))
            {
                result = Serializer.Deserialize<T>(stream);
            }

            return result;
        }
    }
}