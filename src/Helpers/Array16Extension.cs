using CSM.Panels;
using Harmony;
using NLog;
using System.Reflection;

namespace CSM.Helpers
{
    public static class Array16Extension
    {
        public static void RemoveUnused<T>(this Array16<T> arr, uint id)
        {
            ushort[] unusedItems = (ushort[])arr.GetType().GetField("m_unusedItems", AccessTools.all).GetValue(arr);
            FieldInfo unusedCountField = arr.GetType().GetField("m_unusedCount", AccessTools.all);
            uint unusedCount = (uint)unusedCountField.GetValue(arr);

            uint expectedPos = (uint)id - 1;
            // Special case, when array was modified (search for the id)
            if (unusedItems[expectedPos] != id)
            {
                bool found = false;
                for (uint num = 0; num <= unusedCount; num++)
                {
                    if (unusedItems[num] == id)
                    {
                        expectedPos = num;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    // The arrays are no longer in sync
                    LogManager.GetCurrentClassLogger().Error("Array16: Received id already in use. Please restart the multiplayer session!");
                    ChatLogPanel.PrintGameMessage(ChatLogPanel.MessageType.Error, "ID collision. Please restart the multiplayer session.");
                    return;
                }
            }

            unusedItems[expectedPos] = unusedItems[--unusedCount];

            unusedCountField.SetValue(arr, unusedCount);
        }
    }
}