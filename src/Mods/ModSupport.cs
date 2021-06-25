using System;
using System.Collections.Generic;
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
                Connection connectionInstance = (Connection) Activator.CreateInstance(handler);
                if (connectionInstance.ConnectToCSM(SendToAll))
                    Log.Info("Mod connected: " + connectionInstance.name);
                else
                    Log.Warn("Mod failed to connect: " + connectionInstance.name);
            }
        }

        public bool SendToAll(CommandBase command)
        {
            Command.SendToAll(command);
            return false;
        }


    }
}