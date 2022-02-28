using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Plugins;
using CSM.Util;

namespace CSM.Helpers
{
    public static class AssemblyHelper
    {
        private static IEnumerable<Assembly> GetEnabledAssemblies()
        {
            List<Assembly> assemblies = new List<Assembly>();
            foreach (PluginManager.PluginInfo info in Singleton<PluginManager>.instance.GetPluginsInfo())
            {
                if (info.isEnabled)
                {
                    assemblies.AddRange(info.GetAssemblies());
                }                
            }

            return assemblies;
        }
        
        /// <summary>
        /// Searches all the assemblies in the current AppDomain for class definitions that implement the given Type.
        /// Some default assemblies will be skipped to improve performance since. Check @IgnoredAssemblies.
        /// </summary>
        public static IEnumerable<Type> FindClassesInMods(Type typeToSearchFor)
        {
            return GetEnabledAssemblies().SelectMany(assembly => FindImplementationsInAssembly(assembly, typeToSearchFor));
        }

        /// <summary>
        /// Searches CSM Assembly in the current AppDomain for class definitions that implement the given Type.
        /// </summary>
        public static IEnumerable<Type> FindClassesInCSM(Type typeToSearchFor)
        {
            // Only use own assembly, others have to be registered as mods
            Assembly assembly = typeof(CSM).Assembly;

            IEnumerable<Type> handlers = FindImplementationsInAssembly(assembly, typeToSearchFor);
            foreach (Type handler in handlers)
            {
                yield return handler;
            }
        }

        /// <summary>
        /// Finds all the implementations of the given type in the given Assembly.
        /// </summary>
        public static IEnumerable<Type> FindImplementationsInAssembly(Assembly assembly, Type typeToSearchFor)
        {
            Type[] types = Type.EmptyTypes;
            try
            {
                types = assembly.GetTypes();
            }
            catch
            {
                // ignored
            }

            foreach (Type type in types)
            {
                bool isValid = false;
                try
                {
                    isValid = typeToSearchFor.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract;
                }
                catch
                {
                    // ignored
                }

                if (isValid)
                {
                    yield return type;
                }
            }
        }
    }
}
