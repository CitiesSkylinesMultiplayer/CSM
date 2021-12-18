using CSM.API;
using CSM.API.Commands;
using CSM.Commands;
using CSM.Helpers;
using CSM.Util;
using System;
using System.Collections.Generic;

namespace CSM.Mods
{
    class ModSupport
    {
        List<Connection> connectedMods;
        public void registerCommandSenders()
        {
            connectedMods = new List<Connection>();
            IEnumerable<Type> handlers = ReflectionHelper.FindClassesInMods(typeof(Connection));

            foreach (var handler in handlers)
            {
                Connection connectionInstance = (Connection)Activator.CreateInstance(handler);

                if (connectionInstance != null && connectionInstance.ConnectToCSM(SendToAll, SendToServer))
                {
                    Log.Info("Mod connected: " + connectionInstance.name);
                    connectedMods.Add(connectionInstance);
                }
                else if (connectionInstance != null)
                    Log.Warn("Mod failed to connect: " + connectionInstance.name);
                else
                    Log.Warn("Mod failed to instanciate.");
            }
        }

        public void DestroyConnections()
        {
            connectedMods.Clear();
            connectedMods.TrimExcess();
            connectedMods = null;
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
