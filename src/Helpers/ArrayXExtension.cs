using CSM.Panels;
using Harmony;
using NLog;
using System;
using System.Collections.Generic;
using System.Reflection;
using CSM.Localisation;

namespace CSM.Helpers
{
    public static class ArrayXExtension
    {
        public static void RemoveUnused(object arr, uint id)
        {
            if (arr.GetType().Name.StartsWith("Array8"))
            {
                RemoveUnused(arr, (byte) id, "Array8");
            }
            else if (arr.GetType().Name.StartsWith("Array16"))
            {
                RemoveUnused(arr, (ushort) id, "Array16");
            }
            else if (arr.GetType().Name.StartsWith("Array32"))
            {
                RemoveUnused(arr, id, "Array32");
            }
            else
            {
                LogManager.GetCurrentClassLogger().Error($"ArrayXExtension: {arr.GetType()} is not a supported type!");
            }
        }

        private static void RemoveUnused<N>(object arr, N id, string type) where N: IConvertible
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
                    ChatLogPanel.PrintGameMessage(ChatLogPanel.MessageType.Error, $"{Translation.PullTranslation("IDCollision")}");
                    return;
                }
            }

            unusedItems[expectedPos] = unusedItems[--unusedCount];

            unusedCountField.SetValue(arr, unusedCount);
        }
    }
}
