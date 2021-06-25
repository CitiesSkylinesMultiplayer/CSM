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

        public void initModSupport()
        {
            FindHandlersInLoadedAssemblies();

            Log.Info(" ------------- Testing mod support!");
            RegisterHandlers();
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
            Log.Info(" ------------------------------------------ Testing mod support!");
            IEnumerable<Type> handlers = FindHandlersInLoadedAssemblies();
            //RegisterHandlers(handlers);
        }


        /// <summary>
        /// Searches all the assemblies in the current AppDomain, and returns a collection of those that implement the <see cref="IRequestHandler"/> interface.
        /// </summary>
        private IEnumerable<Type> FindHandlersInLoadedAssemblies()
        {
            Log.Info(" ------------------------------------------------------------------------ Testing mod support!");
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Log.Info("Printing out all the assemblies!");
            foreach (var assembly in assemblies)
            {
                Log.Info(assembly.FullName);
                Log.Info("-----");

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

            Log.Info("Printing out all the assemblies!");
            foreach (var type in types)
            {
                //Boolean isValid = false;
                //try
                //{
                //    isValid = typeof(IRequestHandler).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract;
                //}
                //catch { }

                //if (isValid)
                //{
                //    yield return type;
                //}

                Log.Info(" 1 - " + type.FullName);
                Log.Info(" 2 - " + type.AssemblyQualifiedName);
                Log.Info("-----");
                yield return type;
            }
        }

    }

}
