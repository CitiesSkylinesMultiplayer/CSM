using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSM.Server.Util
{
    public class ChatLogPanel
    {


        public enum MessageType
        {
            Normal,
            Warning,
            Error
        }

        /// <summary>
        ///     Prints a chat message to the ChatLogPanel and Chirper.
        /// </summary>
        /// <param name="username">The name of the sending user.</param>
        /// <param name="msg">The message.</param>
        public static void PrintChatMessage(string username, string msg)
        {
            PrintMessage(username, msg);
        }

        /// <summary>
        ///     Prints a game message to the ChatLogPanel and Chirper with MessageType.NORMAL.
        /// </summary>
        /// <param name="msg">The message.</param>
        public static void PrintGameMessage(string msg)
        {
            PrintGameMessage(MessageType.Normal, msg);
        }

        /// <summary>
        ///     Prints a game message to the ChatLogPanel and Chirper.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <param name="msg">The message.</param>
        public static void PrintGameMessage(MessageType type, string msg)
        {
            PrintMessage("CSM", msg);
        }

        private static void PrintMessage(string sender, string msg)
        {
            try
            {
                Console.WriteLine($"<{sender}> {msg}");
            }
            catch (Exception)
            {
                // IGNORE
            }
        }
    }
}
