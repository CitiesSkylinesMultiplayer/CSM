using System;
using System.Collections.Generic;
using System.Reflection;
using CSM.API;
using CSM.API.Commands;
using CSM.Commands;
using CSM.Helpers;
using CSM.Util;

namespace CSM.Mods
{
    class ModSupport
    {

        public void registerCommandSenders()
        {
            IEnumerable<Type> handlers = ReflectionHelper.FindClassesInMods(typeof(Connection));

            foreach (var handler in handlers)
            {
                Connection connectionInstance = (Connection) handler
                    .GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null, null);

                if (connectionInstance != null && connectionInstance.ConnectToCSM(SendToAll, SendToServer))
                    Log.Info("Mod connected: " + connectionInstance.name);
                else if (connectionInstance != null)
                    Log.Warn("Mod failed to connect: " + connectionInstance.name);
                else
                    Log.Warn("Mod failed to instanciate.");
            }
        }

        public bool SendToAll(CommandBase command)
        {
            Command.SendToAll(command);
            return false;
        }
        public bool SendToServer(CommandBase command)
        {
            Command.SendToServer(command);
            return false;
        }
    }
}