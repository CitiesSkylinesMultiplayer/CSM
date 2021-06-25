using System;
using System.Collections.Generic;
using CSM.API.Commands;
using CSM.API.Networking;
using CSM.Helpers;
using LiteNetLib;

namespace CSM.Mods
{
    class ModSupport
    {
        public void initModSupport()
        {
            RegisterHandlers();

            Log.Info("Printing out all Handlers tests!");
            foreach (var handler in _tests)
            {
                Log.Info(handler.Value.Handle(null));
                handler.Value.ConnectToCSM(TransmitCommandToAllClients);

            }
        }

        private void RegisterHandlers()
        {
            IEnumerable<Type> handlers = CommandReflectionHelper.FindClassesByType(typeof(Connection));

            foreach (var handler in handlers)
            {
                Connection connectionInstance = (Connection) Activator.CreateInstance(handler);
                connectionInstance.ConnectToCSM(SendToClient, SendToClient2, SendToClients, SendToOtherClients,
                    SendToServer, SendToAll);
            }
        }

        private IEnumerable<Type> FetchHandlers(Assembly assembly)
        {
            var assemblyName = assembly.GetName().Name;

            // Skip any assemblies that we don't anticipate finding anything in.
            if (IgnoredAssemblies.Contains(assemblyName)) { yield break; }

            Type[] types = new Type[0];
            try
            {
                types = assembly.GetTypes();
            }
            catch { }

            foreach (var type in types)
            {
                Boolean isValid = false;
                try
                {
                    isValid = typeof(ITest).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract;
                }
                catch { }

                if (isValid)
                {
                    yield return type;
                }
            }
        }

        private void RegisterHandlers(IEnumerable<Type> handlers)
        {
            if (handlers == null) { return; }

            if (_tests == null)
            {
                _tests = new Dictionary<string, ITest>();
            }

            foreach (var handler in handlers)
            {
                // Only register handlers that we don't already have an instance of.
                if (_tests.Any(h => h.GetType() == handler))
                {
                    continue;
                }

                ITest handlerInstance = null;
                Boolean exists = false;

                try
                {
                    handlerInstance = (ITest)Activator.CreateInstance(handler);
                    
                    if (handlerInstance == null)
                    {
                        Log.Info(String.Format("Request Handler ({0}) could not be instantiated!", handler.Name));
                        continue;
                    }

                    // Duplicates handlers seem to pass the check above, so now we filter them based on their identifier values, which should work.
                    exists = _tests.Any(obj => obj.Value.HandlerID == handlerInstance.HandlerID);
                }
                catch (Exception ex)
                {
                    Log.Info(ex.ToString());
                }

                if (exists)
                {
                    // TODO: Allow duplicate registrations to occur; previous registration is removed and replaced with a new one?
                    Log.Info(String.Format("Supressing duplicate handler registration for '{0}'", handler.Name));
                }
                else
                {
                    _tests.Add(handlerInstance.name, handlerInstance);
                    Log.Info(String.Format("Added Request Handler: {0}", handler.FullName));
                }
            }
        }

        public bool TransmitCommandToAllClients(CommandBase command)
        {
            Command.SendToAll(command);

            _tests[command.Name].Handle(command.Data);
        }

        public bool TransmitCommandToAllClients(string name, byte[] data)
        {
            Command.SendToAll(new ExternalAPICommand
            {
                Name = name,
                Data = data
            });
            return true;
        }
    }
}