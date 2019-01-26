using ColossalFramework.UI;
using CSM.Commands;
using CSM.Common;
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

        private float _initialOpacity;

        // Class logger
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        // Name of this component
        public const string NAME = "MPChatLogPanel";

        private readonly List<ChatCommand> _chatCommands;

        public enum MessageType
        {
            Normal,
            Warning,
            Error
        }

        public ChatLogPanel()
        {
            _chatCommands = new List<ChatCommand>
            {
                new ChatCommand("help", "Displays information about which commands can be displayed.", (command) =>
                {
                    foreach (var c in _chatCommands)
                    {
                        PrintGameMessage($"/{c.Command} : {c.Description}");
                    };
                }),
                new ChatCommand("version", "Displays version information about the mod and game.", (command) =>
                {
                    PrintGameMessage("Mod Version  : " + Assembly.GetAssembly(typeof(CSM)).GetName().Version.ToString());
                    PrintGameMessage("Game Version : " + BuildConfig.applicationVersion);
                }),
                new ChatCommand("support", "Display support links for the mod.", (command) =>
                {
                    PrintGameMessage("GitHub : https://github.com/DominicMaas/Tango");
                    PrintGameMessage("Discord : https://www.patreon.com/CSM_MultiplayerMod");
                    PrintGameMessage("Steam Workshop : https://steamcommunity.com/sharedfiles/filedetails/?id=1558438291");
                }),
                new ChatCommand("players", "Displays a list of players connected to the server", (command) =>
                {
                    if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.None)
                    {
                        PrintGameMessage("You are not hosting or connected to any servers.");
                        return;
                    }

                    foreach (var player in MultiplayerManager.Instance.PlayerList)
                    {
                        PrintGameMessage(player);
                    }
                }),
                new ChatCommand("hide", "Hides the chat console. Can be displayed again by pressing the '`' key.", (command) =>
                {
                    opacity = 0.0f;
                }),
                new ChatCommand("donate", "Find out how to support the mod developers", (command) =>
                {
                    PrintGameMessage("Want to help support the mod?");
                    PrintGameMessage("Help develop the mod here: https://github.com/DominicMaas/Tango");
                    PrintGameMessage("Donate to the developers here: https://www.patreon.com/CSM_MultiplayerMod");
                }),
                new ChatCommand("clear", "Clear everything from the chat log.", (command) =>
                {
                    _messageBox.items = new string[0];
                })
            };
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
                // Get the chat log panel
                var panel = UIView.GetAView().FindUIComponent(ChatLogPanel.NAME) as ChatLogPanel;

                // Add the chat log
                panel?.AddGameMessage(type, msg);
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
                // Get the chat log panel
                var panel = UIView.GetAView().FindUIComponent(ChatLogPanel.NAME) as ChatLogPanel;

                // Add the chat log
                panel?.AddChatMessage(username, msg);
            });
        }

        public override void Update()
        {
            // Toggle the chat when the tidle key is pressed.
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                opacity = opacity == _initialOpacity ? 0.0f : _initialOpacity;
            }

            base.Update();
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
            name = ChatLogPanel.NAME;
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
            _messageBox.multilineItems = true;

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

            _initialOpacity = opacity;

            PrintGameMessage("Welcome to Cities: Skylines Multiplayer!");
            PrintGameMessage("Press the ~ (tilde) key to show or hide the chat.");
            PrintGameMessage("Join our discord server at: https://discord.gg/RjACPhd");
            PrintGameMessage("Type '/help' to see a list of commands and usage.");
            PrintGameMessage("Type '/support' to find out where to report bugs and get help.");

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
                    var command = _chatCommands.Find(x => x.Command == text.TrimStart('/'));
                    if (command == null)
                    {
                        PrintGameMessage(MessageType.Warning, $"'{text.TrimStart('/')}' is not a valid command.");
                        return;
                    }

                    // Run the command
                    command.Action.Invoke(text.TrimStart('/'));

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