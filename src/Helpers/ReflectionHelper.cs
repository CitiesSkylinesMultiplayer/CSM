using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSM.Helpers
{
    public static class ReflectionHelper
    {
        public static BindingFlags AllAccessFlags =
            BindingFlags.Public
            | BindingFlags.NonPublic
            | BindingFlags.Instance
            | BindingFlags.Static;

        /// <summary>
        /// List of Assemblies we don't want to search references in.
        /// This improves performance as we don't need to dig into them.
        /// </summary>
        private static readonly List<String> IgnoredAssemblies = new List<String>
        {
            "Anonymously Hosted DynamicMethods Assembly",
            "Assembly-CSharp",
            "Assembly-CSharp-firstpass",
            "CSM.API",
            "ColossalManaged",
            "HarmonySharedState",
            "ICSharpCode.SharpZipLib",
            "ICities",
            "LiteNetLib",
            "mscorlib",
            "Mono.Security",
            "NLog",
            "Open.Nat",
            "protobuf-net",
            "System.Threading",
            "System",
            "System.Configuration",
            "System.Core",
            "System.Xml",
            "UnityEngine",
            "UnityEngine.UI",
            "UnityEngine.Networking",
            "0Harmony",
        };

        public const String CSM_PACKAGE_NAME = "CSM";

        public static int GetEnumValue(Type enumType, string value)
        {
            int i = 0;
            foreach (string name in Enum.GetNames(enumType))
            {
                if (name.Equals(value))
                {
                    return (int) Enum.GetValues(enumType).GetValue(i);
                }

                i++;
            }

            return 0;
        }

        public static T Call<T>(Type type, string name, params object[] param)
        {
            return (T) Call(type, name, param);
        }

        public static object Call(Type type, string name, params object[] param)
        {
            return type.GetMethod(name, AllAccessFlags, null, param.Select(p => p.GetType()).ToArray(), null)
                ?.Invoke(null, param);
        }

        public static T Call<T>(object obj, string name, params object[] param)
        {
            return (T) Call(obj, name, param);
        }

        public static object Call(object obj, string name, params object[] param)
        {
            return obj.GetType().GetMethod(name, AllAccessFlags, null, param.Select(p => p.GetType()).ToArray(), null)
                ?.Invoke(obj, param);
        }

        public static void SetAttr(object obj, string attribute, object value)
        {
            obj.GetType().GetField(attribute, AllAccessFlags)?.SetValue(obj, value);
        }

        public static T GetAttr<T>(object obj, string attribute)
        {
            return (T) GetAttr(obj, attribute);
        }

        public static object GetAttr(object obj, string attribute)
        {
            return obj.GetType().GetField(attribute, AllAccessFlags)?.GetValue(obj);
        }

        public static T GetProp<T>(object obj, string property)
        {
            return (T) GetProp(obj, property);
        }

        public static object GetProp(object obj, string property)
        {
            return obj.GetType().GetProperty(property, AllAccessFlags)?.GetValue(obj, null);
        }

        public static MethodBase GetIteratorTargetMethod(Type container, string itName, out Type iteratorType)
        {
            iteratorType = container.GetNestedType(itName, AllAccessFlags);
            return iteratorType.GetMethod("MoveNext", AllAccessFlags);
        }

        /// <summary>
        /// Searches all the assemblies in the current AppDomain for class definitions that implement the given Type.
        /// Some default assemblies will be skipped to improve performance since. Check @IgnoredAssemblies.
        /// </summary>
        public static IEnumerable<Type> FindClassesInMods(Type typeToSearchFor)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var assemblyName = assembly.GetName().Name;

                // Skip any assemblies that we don't anticipate finding anything in.
                if (IgnoredAssemblies.Contains(assemblyName))
                    continue;

                var handlers = FindImplementationsInAssembly(assembly, typeToSearchFor);
                foreach (var handler in handlers)
                {
                    yield return handler;
                }
            }
        }

        /// <summary>
        /// Searches CSM Assembly in the current AppDomain for class definitions that implement the given Type.
        /// Some default assemblies will be skipped to improve performance since. Check @IgnoredAssemblies.
        /// </summary>
        public static IEnumerable<Type> FindClassInCSM(Type typeToSearchFor)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assembly = assemblies.First(x => x.GetName().Name == CSM_PACKAGE_NAME);

            var handlers = FindImplementationsInAssembly(assembly, typeToSearchFor);
            foreach (var handler in handlers)
            {
                yield return handler;
            }
        }

        /// <summary>
        /// Finds all the implementations of the given type in the given Assembly.
        /// </summary>
        private static IEnumerable<Type> FindImplementationsInAssembly(Assembly assembly, Type typeToSearchFor)
        {
            Type[] types = new Type[0];
            try
            {
                types = assembly.GetTypes();
            }
            catch
            {
            }

            foreach (var type in types)
            {
                Boolean isValid = false;
                try
                {
                    isValid = typeToSearchFor.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract;
                }
                catch
                {
                }

                if (isValid)
                {
                    yield return type;
                }
            }
        }
    }
}
