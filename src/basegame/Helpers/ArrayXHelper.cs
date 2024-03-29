﻿using System;
using System.Collections.Generic;
using System.Reflection;
using CSM.API;
using CSM.API.Helpers;

namespace CSM.BaseGame.Helpers
{
    public static class ArrayXExtension
    {
        public static void RemoveUnused(object arr, uint id)
        {
            if (arr.GetType().Name.StartsWith("Array8"))
            {
                RemoveUnused(arr, (byte)id, "Array8");
            }
            else if (arr.GetType().Name.StartsWith("Array16"))
            {
                RemoveUnused(arr, (ushort)id, "Array16");
            }
            else if (arr.GetType().Name.StartsWith("Array32"))
            {
                RemoveUnused(arr, id, "Array32");
            }
            else
            {
                Log.Error($"ArrayXExtension: {arr.GetType()} is not a supported type!");
            }
        }

        private static void RemoveUnused<N>(object arr, N id, string type) where N : IConvertible
        {
            N[] unusedItems = (N[])arr.GetType().GetField("m_unusedItems", ReflectionHelper.AllAccessFlags).GetValue(arr);
            FieldInfo unusedCountField = arr.GetType().GetField("m_unusedCount", ReflectionHelper.AllAccessFlags);
            uint unusedCount = (uint)unusedCountField.GetValue(arr);

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
                    Log.Error($"{type}: Received id {id} already in use. Please restart the multiplayer session!");
                    Chat.Instance.PrintGameMessage(Chat.MessageType.Error, "ID collision. Please restart the multiplayer session.");
                    return;
                }
            }

            unusedItems[expectedPos] = unusedItems[--unusedCount];

            unusedCountField.SetValue(arr, unusedCount);
        }
    }
}
