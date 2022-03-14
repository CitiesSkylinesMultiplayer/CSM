using System;
using System.Linq;
using System.Reflection;

namespace CSM.API.Helpers
{
    public static class ReflectionHelper
    {
        public static BindingFlags AllAccessFlags =
            BindingFlags.Public
            | BindingFlags.NonPublic
            | BindingFlags.Instance
            | BindingFlags.Static;

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

        /// <summary>
        ///     Call a method through reflection.
        ///     This method infers parameter types, if
        ///     you need to pass null, use the Call method
        ///     with explicit parameter types.
        /// </summary>
        /// <param name="obj">The object to call the method on.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="param">The non-null parameters.</param>
        /// <returns>The return value.</returns>
        public static object Call(object obj, string name, params object[] param)
        {
            return Call(obj, name, param.Select(p => p.GetType()).ToArray(), param);
        }

        public static object Call(object obj, string name, Type[] types, params object[] param)
        {
            return obj.GetType().GetMethod(name, AllAccessFlags, null, types, null)
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
    }
}
