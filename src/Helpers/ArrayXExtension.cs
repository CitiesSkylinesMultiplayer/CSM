using CSM.Panels;
using Harmony;
using NLog;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSM.Helpers
{
    public static class ArrayXExtension
    {
        public static void RemoveUnused<T>(this Array8<T> arr, byte id)
        {
            ArrayXHelper<Array8<T>, byte>.RemoveUnused(arr, id, "Array8");
        }

        public static void RemoveUnused<T>(this Array16<T> arr, ushort id)
        {
            ArrayXHelper<Array16<T>, ushort>.RemoveUnused(arr, id, "Array16");
        }

        public static void RemoveUnused<T>(this Array32<T> arr, uint id)
        {
            ArrayXHelper<Array32<T>, uint>.RemoveUnused(arr, id, "Array32");
        }

        private static class ArrayXHelper<A, N> where N : IConvertible
        {
            public static void RemoveUnused(A arr, N id, string type)
            {
                N[] unusedItems = (N[]) arr.GetType().GetField("m_unusedItems", AccessTools.all).GetValue(arr);
                FieldInfo unusedCountField = arr.GetType().GetField("m_unusedCount", AccessTools.all);
                uint unusedCount = (uint) unusedCountField.GetValue(arr);

                uint expectedPos = Convert.ToUInt32(id) - 1;
                // Special case, when array was modified (search for the id)
                if (expectedPos >= unusedCount || !EqualityComparer<N>.Default.Equals(unusedItems[expectedPos], id))
                {
                    bool found = false;
                    for (uint num = 0; num < unusedCount; num++)
                    {
                        if (EqualityComparer<N>.Default.Equals(unusedItems[num], id))
                        {
                            expectedPos = num;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        // The arrays are no longer in sync
                        LogManager.GetCurrentClassLogger().Error($"{type}: Received id {id} already in use. Please restart the multiplayer session!");
                        ChatLogPanel.PrintGameMessage(ChatLogPanel.MessageType.Error, "ID collision. Please restart the multiplayer session.");
                        return;
                    }
                }

                unusedItems[expectedPos] = unusedItems[--unusedCount];

                unusedCountField.SetValue(arr, unusedCount);
            }
        }
    }
}
