using System;
using System.Reflection;
using System.Linq;

namespace CSM.Common
{
    public class ReflectionHelper
    {
        public static BindingFlags AllAccessFlags = 
              BindingFlags.Public
            | BindingFlags.NonPublic
            | BindingFlags.Instance
            | BindingFlags.Static;

        public static object GetEnumValue(Type enumType, string value)
        {
            int i = 0;
            foreach (string name in Enum.GetNames(enumType))
            {
                if (name.Equals(value))
                {
                    return Enum.GetValues(enumType).GetValue(i);
                }
                i++;
            }

            return null;
        }

        public static T Call<T>(object obj, string name, params object[] param)
        {
            return (T) Call(obj, name, param);
        }

        public static object Call(object obj, string name, params object[] param)
        {
            return obj.GetType().GetMethod(name, AllAccessFlags, null, param.Select(p => p.GetType()).ToArray(), null).Invoke(obj, param);
        }

        public static void SetAttr(object obj, string attribute, object value)
        {
            obj.GetType().GetField(attribute, AllAccessFlags).SetValue(obj, value);
        }

        public static T GetAttr<T>(object obj, string attribute)
        {
            return (T) GetAttr(obj, attribute);
        }

        public static object GetAttr(object obj, string attribute)
        {
            return obj.GetType().GetField(attribute, AllAccessFlags).GetValue(obj);
        }
    }
}
