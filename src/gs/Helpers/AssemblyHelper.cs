using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSM.GS.Helpers
{
    public static class AssemblyHelper
    {
        /// <summary>
        /// Searches CSM API server Assembly in the current AppDomain for class definitions that implement the given Type.
        /// </summary>
        public static IEnumerable<Type> FindClassesInAssembly(Type typeToSearchFor)
        {
            // Only use own assembly, others have to be registered as mods
            Assembly assembly = typeof(AssemblyHelper).Assembly;

            IEnumerable<Type> handlers = FindImplementationsInAssembly(assembly, typeToSearchFor);
            foreach (Type handler in handlers)
            {
                yield return handler;
            }
        }

        /// <summary>
        /// Finds all the implementations of the given type in the given Assembly.
        /// </summary>
        private static IEnumerable<Type> FindImplementationsInAssembly(Assembly assembly, Type typeToSearchFor)
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
