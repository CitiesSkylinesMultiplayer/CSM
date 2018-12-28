using ColossalFramework.UI;
using CSM.Commands;
using CSM.Helpers;
using CSM.Networking;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CSM.Panels
{
    /// <summary>
    ///     Displays a chat on the users screen. Allows a user to send messages
    ///     to other players and view important events such as server startup and
    ///     connections.
    /// </summary>
    public class ChatLogPanel : UIPanel
    {
        private UIListBox _messageBox;
        private UITextField _chatText;

        public enum MessageType
        {
            Normal,
            Warning,
            Error
        }

        /// <summary>
        ///     Prints a game message to the ChatLogPanel with MessageType.NORMAL.
        /// </summary>
        /// <param name="msg">The message.</param>
        public static void PrintGameMessage(string msg)
        {
            PrintGameMessage(MessageType.Normal, msg);
        }

        /// <summary>
        ///     Prints a game message to the ChatLogPanel.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <param name="msg">The message.</param>
        public static void PrintGameMessage(MessageType type, string msg)
        {
            SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
            {
                var panel = (ChatLogPanel) UIView.GetAView().FindUIComponent("MPChatLogPanel");
                panel?.AddGameMessage(type, msg); // The panel sometimes seems to be null?
            });
        }

        /// <summary>
        ///     Prints a chat message to the ChatLogPanel.
        /// </summary>
        /// <param name="username">The name of the sending user.</param>
        /// <param name="msg">The message.</param>
        public static void PrintChatMessage(string username, string msg)
        {
            SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
            {
                var panel = (ChatLogPanel) UIView.GetAView().FindUIComponent("MPChatLogPanel");
                panel?.AddChatMessage(username, msg);
            });
        }

        public override void Start()
        {
            // Generates the following UI:
            // |---------------|
            // |               | <-- _messageBox
            // |               |
            // |---------------|
            // |               | <-- _chatText
            // |---------------|

            backgroundSprite = "GenericPanel";
            name = "MPChatLogPanel";
            color = new Color32(22, 22, 22, 240);

            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            // Grab the view for calculating width and height of game
            var view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(10.0f, view.fixedHeight - 440.0f);

            width = 500;
            height = 300;

            // Create the message box
            _messageBox = (UIListBox)AddUIComponent(typeof(UIListBox));
            _messageBox.isVisible = true;
            _messageBox.isEnabled = true;
            _messageBox.width = 480;
            _messageBox.height = 240;
            _messageBox.position = new Vector2(10, -10);
            _messageBox.multilineItems = true;
            _messageBox.textScale = 0.8f;
            _messageBox.itemHeight = 20;

            // Create the message text box (used for sending messages)
            _chatText = (UITextField)AddUIComponent(typeof(UITextField));
            _chatText.width = width;
            _chatText.height = 30;
            _chatText.position = new Vector2(0, -270);
            _chatText.atlas = UiHelpers.GetAtlas("Ingame");
            _chatText.normalBgSprite = "TextFieldPanelHovered";
            _chatText.builtinKeyNavigation = true;
            _chatText.isInteractive = true;
            _chatText.readOnly = false;
            _chatText.horizontalAlignment = UIHorizontalAlignment.Left;
            _chatText.eventKeyDown += OnChatKeyDown;
            _chatText.textColor = new Color32(0, 0, 0, 255);
            _chatText.padding = new RectOffset(6, 6, 6, 6);
            _chatText.selectionSprite = "EmptySprite";
            base.Start();
        }

        private void OnChatKeyDown(UIComponent component, UIKeyEventParameter eventParam)
        {
            // Don't run this code if the user has typed nothing in
            if (string.IsNullOrEmpty(_chatText.text))
                return;

            // Run this code when the user presses the enter key
            if (eventParam.keycode == KeyCode.Return || eventParam.keycode == KeyCode.KeypadEnter)
            {
                // Get and clear the text
                var text = _chatText.text;
                _chatText.text = string.Empty;

                // If a command, parse it
                if (text.StartsWith("/"))
                {
                    ParseCommand(text);
                    return;
                }

                // If not connected to a server / hosting a server, tell the user and return
                if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.None)
                {
                    PrintGameMessage(MessageType.Warning, "You can only use the chat feature when hosting or connected.");
                    return;
                }

                // Get the player name
                var playerName = "Player";

                if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
                {
                    playerName = MultiplayerManager.Instance.CurrentClient.Config.Username;
                }
                else if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
                {
                    playerName = MultiplayerManager.Instance.CurrentServer.Config.Username;
                }

                // Build and send this message
                var message = new ChatMessageCommand
                {
                    Username = playerName,
                    Message = text
                };

                Command.SendToAll(message);

                // Add the message to the chat UI
                PrintChatMessage(playerName, text);
            }
        }

        private void ParseCommand(string command)
        {
            switch (command.TrimStart('/'))
            {
                case "version":
                    PrintGameMessage("Mod Version  : " + Assembly.GetAssembly(typeof(CSM)).GetName().Version.ToString());
                    PrintGameMessage("Game Version : " + BuildConfig.applicationVersion);
                    break;

                case "players":
                    foreach (var player in MultiplayerManager.Instance.PlayerList)
                    {
                        PrintGameMessage(player);
                    }
                    break;

                default:
                    PrintGameMessage(MessageType.Warning, $"{command.TrimStart('/')} is not a valid command.");
                    break;
            }
        }

        private void AddMessage(MessageType type, string message)
        {
            // Game console
            var existingItems = new List<string>();
            existingItems.AddRange(_messageBox.items);
            existingItems.Add(message);

            _messageBox.items = existingItems.ToArray();

            // Scroll to the bottom
            _messageBox.scrollPosition = (_messageBox.items.Length * 20) + 10;
        }

        private void AddChatMessage(string username, string message)
        {
            AddMessage(MessageType.Normal, $"<{username}> {message}");
        }

        private void AddGameMessage(MessageType type, string message)
        {
            AddMessage(type, $"<CSM> {message}");
        }
    }
}
