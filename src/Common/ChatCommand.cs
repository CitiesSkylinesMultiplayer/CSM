using System;

namespace CSM.Common
{
    /// <summary>
    ///     Represents a holder for a chat command
    /// </summary>
    public class ChatCommand
    {
        public ChatCommand(string command, string description, Action<string> action)
        {
            Command = command;
            Description = description;
            Action = action;
        }

        /// <summary>
        ///     The text that the user must type
        ///     to trigger this command.
        /// </summary>
        public string Command { get; }

        /// <summary>
        ///     Help text about this command
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     The method to run when the user types this
        ///     command, takes in the command text.
        /// </summary>
        public Action<string> Action { get; }
    }
}
