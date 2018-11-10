using ColossalFramework.UI;
using CSM.Panels;
using System;
using System.IO;
using UnityEngine;

namespace CSM.Common
{
    /// <summary>
    ///     Use this class to perform all logging in the application. There are multiple
    ///     levels of logging, console, file, and chat.
    /// </summary>
    public static class LogManager
    {
        /// <summary>
        ///     Log a message to the Unity / Cities Skylines console and log file. This
        ///     does not log a message to the chat (so useful for debugging). The log file
        ///     is saved under the Cities Skylines install folder (multiplayer-log.txt).
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogDebug(string message)
        {
            // Unity / Cities Skylines
            Debug.Log(message);

            // Write to log file
            using (var writer = File.AppendText("multiplayer-log.txt"))
            {
                writer.WriteLine($"[{DateTime.Now.ToLongTimeString()}] [Debug] {message}");
            }
        }

        /// <summary>
        ///     Log a message to the Unity / Cities Skylines console, log file and chat box.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogMessage(string message)
        {
            // Unity / Cities Skylines
            Debug.Log(message);

            // Game console
            var chatLog = UIView.GetAView().FindUIComponent<ChatLogPanel>("MPChatLogPanel");
            chatLog?.AddMessage(ChatLogPanel.MessageType.Normal, message);

            // Write to log file
            using (var writer = File.AppendText("multiplayer-log.txt"))
            {
                writer.WriteLine($"[{DateTime.Now.ToLongTimeString()}] [Message] {message}");
            }
        }

        /// <summary>
        ///     Log a message to the Unity / Cities Skylines console, log file and chat box.
        ///     The message will be highlighted yellow and prefixed with [Warning] in the log file.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogWarning(string message)
        {
            // Unity / Cities Skylines
            Debug.LogWarning(message);

            // Game console
            var chatLog = UIView.GetAView().FindUIComponent<ChatLogPanel>("MPChatLogPanel");
            chatLog?.AddMessage(ChatLogPanel.MessageType.Warning, message);

            // Write to log file
            using (var writer = File.AppendText("multiplayer-log.txt"))
            {
                writer.WriteLine($"[{DateTime.Now.ToLongTimeString()}] [Warning] {message}");
            }
        }

        /// <summary>
        ///     Log a message to the Unity / Cities Skylines console, log file and chat box.
        ///     The message will be highlighted red and prefixed with [Error] in the log file.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogError(string message)
        {
            // Unity / Cities Skylines
            Debug.LogError(message);

            // Game console
            var chatLog = UIView.GetAView().FindUIComponent<ChatLogPanel>("MPChatLogPanel");
            chatLog?.AddMessage(ChatLogPanel.MessageType.Error, message);

            // Write to log file
            using (var writer = File.AppendText("multiplayer-log.txt"))
            {
                writer.WriteLine($"[{DateTime.Now.ToLongTimeString()}] [Error] {message}");
            }
        }
    }
}