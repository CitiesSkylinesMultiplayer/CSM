using System;
using System.Collections.Generic;
using System.Reflection;
using CSM.API.Commands;
using CSM.API;
using CSM.Commands;
using CSM.Helpers;
using CSM.Util;

namespace CSM.Mods
{
    class ModSupport
    {
        public void initModSupport()
        {
            RegisterHandlers();
        }

        private void RegisterHandlers()
        {
            IEnumerable<Type> handlers = CommandReflectionHelper.FindClassesByType(typeof(Connection));

            foreach (var handler in handlers)
            {
                Connection connectionInstance = (Connection) handler.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null, null);
                
                if (connectionInstance != null && connectionInstance.ConnectToCSM(SendToAll))
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


    }
}