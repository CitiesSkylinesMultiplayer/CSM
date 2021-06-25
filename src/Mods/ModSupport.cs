using CSM.API;
using CSM.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CSM.Mods
{

    class ModSupport
    {

        private List<ITest> _tests;

        public void initModSupport()
        {
            RegisterHandlers();

            Log.Info("Printing out all Handlers tests!");
            foreach (var handler in _tests)
            {
                Log.Info(handler.Handle());
            }
        }

        // Not required, but prevents a number of spurious entries from making it to the log file.
        private static readonly List<String> IgnoredAssemblies = new List<String>
        {
            "Anonymously Hosted DynamicMethods Assembly",
            "Assembly-CSharp",
            "Assembly-CSharp-firstpass",
            "Assembly-UnityScript-firstpass",
            "Boo.Lang",
            "ColossalManaged",
            "ICSharpCode.SharpZipLib",
            "ICities",
            "Mono.Security",
            "mscorlib",
            "System",
            "System.Configuration",
            "System.Core",
            "System.Xml",
            "UnityEngine",
            "UnityEngine.UI",
        };

        /// <summary>
        /// Searches all the assemblies in the current AppDomain for class definitions that implement the <see cref="IRequestHandler"/> interface.  Those classes are instantiated and registered as request handlers.
        /// </summary>
        private void RegisterHandlers()
        {
            IEnumerable<Type> handlers = FindHandlersInLoadedAssemblies();
            RegisterHandlers(handlers);
        }


        /// <summary>
        /// Searches all the assemblies in the current AppDomain, and returns a collection of those that implement the <see cref="IRequestHandler"/> interface.
        /// </summary>
        private IEnumerable<Type> FindHandlersInLoadedAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var handlers = FetchHandlers(assembly);
                foreach (var handler in handlers)
                {
                    yield return handler;
                }
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
                _tests = new List<ITest>();
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
                    exists = _tests.Any(obj => obj.HandlerID == handlerInstance.HandlerID);
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
                    _tests.Add(handlerInstance);
                    Log.Info(String.Format("Added Request Handler: {0}", handler.FullName));
                }
            }
        }

        public void ModCommandRecieved()
        {

        }

    }

}
