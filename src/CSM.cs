using ColossalFramework.UI;
using CSM.Panels;
using UnityEngine;

namespace CSM
{
    public class CSM : ICities.IUserMod
    {
        public string Name => "CSM";

        public string Description => "Multiplayer mod for Cities: Skylines.";

        /// <summary>
        ///     Log a message to the console.
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            // Console
            Debug.Log($"[CSM] {message}");

            // Game console
            var chatLog = UIView.GetAView().FindUIComponent<ChatLogPanel>("MPChatLogPanel");
            chatLog?.AddMessage($"[CSM] {message}");
        }
    }
}