using System.IO;
using CSM.API;
using CSM.GS.Commands.Data.ApiServer;
using CSM.GS.Commands.Handler.ApiServer;
using LiteNetLib;

namespace CSM.GS.Commands
{
    public static class CommandReceiver
    {
        /// <summary>
        ///     This method is used to parse an incoming message from the API server
        ///     and execute the appropriate action.
        /// </summary>
        /// <param name="reader">The incoming packet including the command type byte.</param>
        public static void Parse(NetPacketReader reader)
        {
            ApiCommandBase cmd = Deserialize(reader.GetRemainingBytes());

            Log.Debug($"Received {cmd.GetType().Name}");

            ApiCommandHandler handler = ApiCommand.Instance.GetApiCommandHandler(cmd.GetType());
            if (handler == null)
            {
                Log.Error($"API command {cmd.GetType().Name} not found!");
                return;
            }

            handler.Parse(cmd);
        }

        /// <summary>
        ///     Deserialize the command from a byte array.
        /// </summary>
        /// <param name="message">A byte array of the message</param>
        /// <returns>The deserialized command.</returns>
        private static ApiCommandBase Deserialize(byte[] message)
        {
            ApiCommandBase result;

            using (MemoryStream stream = new MemoryStream(message))
            {
                result = (ApiCommandBase)ApiCommand.Instance.ApiModel.Deserialize(stream, null, typeof(ApiCommandBase));
            }

            return result;
        }
    }
}
