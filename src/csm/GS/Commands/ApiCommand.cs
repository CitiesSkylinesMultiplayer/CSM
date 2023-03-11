using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CSM.API;
using CSM.API.Commands;
using CSM.GS.Commands.Data.ApiServer;
using CSM.GS.Commands.Handler.ApiServer;
using CSM.Helpers;
using CSM.Networking;
using ProtoBuf.Meta;

namespace CSM.GS.Commands
{
    public class ApiCommand
    {
        public static ApiCommand Instance;

        private readonly Dictionary<Type, ApiCommandHandler> _apiCmdMapping = new Dictionary<Type, ApiCommandHandler>();

        public TypeModel ApiModel { get; private set; }

        /// <summary>
        ///     This method is used to send a command to the API server.
        ///     Does only work if the current game acts as a server.
        /// </summary>
        /// <param name="command">The command to send.</param>
        public void SendToApiServer(ApiCommandBase command)
        {
            if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
                MultiplayerManager.Instance.CurrentServer.SendToApiServer(command);
        }

        /// <summary>
        ///     This method is used to get the handler of given API command.
        /// </summary>
        /// <param name="commandType">The Type of a ApiCommandBase subclass.</param>
        /// <returns>The handler for the given command.</returns>
        public ApiCommandHandler GetApiCommandHandler(Type commandType)
        {
            _apiCmdMapping.TryGetValue(commandType, out ApiCommandHandler handler);
            return handler;
        }

        /// <summary>
        ///     Serializes the command into a byte array for sending over the network.
        /// </summary>
        /// <returns>A byte array containing the message.</returns>
        public static byte[] Serialize(ApiCommandBase cmd)
        {
            byte[] result;

            using (MemoryStream stream = new MemoryStream())
            {
                Instance.ApiModel.Serialize(stream, cmd);
                result = stream.ToArray();
            }

            return result;
        }

        public void RefreshModel()
        {
            _apiCmdMapping.Clear();
            try
            {
                // First, setup protocol model for the API server:
                Type[] apiHandlers = AssemblyHelper.FindClassesInCSM(typeof(ApiCommandHandler)).ToArray();
                Log.Info($"Initializing API data model with {apiHandlers.Length} commands...");
                // Create a protobuf model
                RuntimeTypeModel apiModel = RuntimeTypeModel.Create();
                // Add base command to the protobuf model with all attributes
                apiModel.Add(typeof(ApiCommandBase), true);
                MetaType baseApiCmd = apiModel[typeof(ApiCommandBase)];

                // Lowest id of the subclasses
                int id = 100;

                // Create instances of the handlers, initialize mappings and register command subclasses in the protobuf model
                foreach (Type type in apiHandlers)
                {
                    ApiCommandHandler handler = (ApiCommandHandler)Activator.CreateInstance(type);
                    _apiCmdMapping.Add(handler.GetDataType(), handler);

                    // Add subtype to the protobuf model with all attributes
                    baseApiCmd.AddSubType(id, handler.GetDataType());
                    apiModel.Add(handler.GetDataType(), true);

                    id++;
                }

                // Compile the protobuf model
                apiModel.CompileInPlace();

                ApiModel = apiModel;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to initialize data model", ex);
            }
        }
    }
}
