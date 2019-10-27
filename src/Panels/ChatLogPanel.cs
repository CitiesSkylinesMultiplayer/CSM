using System;
using ColossalFramework.UI;
using CSM.Commands;
using CSM.Helpers;
using CSM.Networking;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CSM.Commands.Data.Internal;
using CSM.Container;
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
        private UILabel _messageBox;
        private UILabel _title;
        private UITextField _chatText;
        private UIResizeHandle _resize;
        private UIScrollablePanel _scrollablepanel;
        private UIScrollbar _scrollbar;
        private UISlicedSprite _trackingsprite;
        private UISlicedSprite _trackingthumb;
        private UIDragHandle _draghandle;

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
                    foreach (ChatCommand c in _chatCommands)
                    {
                        PrintGameMessage($"/{c.Command} : {c.Description}");
                    }
                }),
                new ChatCommand("version", "Displays version information about the mod and game.", (command) =>
                {
                    PrintGameMessage("Mod Version  : " + Assembly.GetAssembly(typeof(CSM)).GetName().Version);
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

                    foreach (string player in MultiplayerManager.Instance.PlayerList)
                    {
                        PrintGameMessage(player);
                    }
                }),
                new ChatCommand("hide-chat", "Hides the chat console. Can be displayed again by pressing the '`' key.", (command) =>
                {
                    isVisible = !isVisible;
                }),
                new ChatCommand("donate", "Find out how to support the mod developers", (command) =>
                {
                    PrintGameMessage("Want to help support the mod?");
                    PrintGameMessage("Help develop the mod here: https://github.com/DominicMaas/Tango");
                    PrintGameMessage("Donate to the developers here: https://www.patreon.com/CSM_MultiplayerMod");
                }),
                new ChatCommand("clear", "Clear everything from the chat log.", (command) =>
                {
                    _messageBox.text = "";
                }),
                new ChatCommand("open-log", "Opens the multiplayer log.", (command) =>
                {
                    Process.Start(Path.GetFullPath(".") + "/multiplayer-logs/log-current.txt");
                })
            };
        }

        public override void Update()
        {
            // Toggle the chat when the tidle key is pressed.
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                isVisible = !isVisible;
            }

            // Gain focus on chat when the tab key is pressed.
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!_chatText.hasFocus && _chatText.isVisible)
                {
                    _chatText.Focus();
                }
            }

            base.Update();
        }

        public override void Start()
        {
            // Generates the following UI:
            // /NAME-----------\ <-- UIDragHandle
            // |---------------|-|
            // |               | |<-- _messageBox, _getscrollablepanel
            // |               | |
            // |---------------| |
            // |               | |<-- _chatText
            // |---------------|-|
            //                 |-|<-- _resize
            //                  ^
            //                  ¦-- _scrollbar, _trackingsprite, _trackingthumb

            backgroundSprite = "GenericPanel";
            name = "ChatLogPanel";
            color = new Color32(22, 22, 22, 240);

            // Activates the dragging of the window
            _draghandle = AddUIComponent<UIDragHandle>();
            _draghandle.name = "ChatLogPanelDragHandle";

            // Grab the view for calculating width and height of game
            UIView view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(10.0f, view.fixedHeight - 440.0f);

            width = 500;
            height = 310;
            minimumSize = new Vector2(300, 310);

            // Add resize component
            _resize = AddUIComponent<UIResizeHandle>();
            _resize.position = new Vector2((width - 20), (-height + 10));
            _resize.width = 20f;
            _resize.height = 20f;
            _resize.color = new Color32(255, 255, 255, 255);
            _resize.backgroundSprite = "GenericTabPressed";
            _resize.name = "ChatLogPanelResize";

            // Add scrollable panel component
            _scrollablepanel = AddUIComponent<UIScrollablePanel>();
            _scrollablepanel.width = 490;
            _scrollablepanel.height = 240;
            _scrollablepanel.position = new Vector2(10, -30);
            _scrollablepanel.clipChildren = true;
            _scrollablepanel.name = "ChatLogPanelScrollablePanel";

            // Add title
            _title = AddUIComponent<UILabel>();
            _title.position = new Vector2(10, -5);
            _title.text = "Multiplayer Chat";
            _title.textScale = 0.8f;
            _title.autoSize = true;
            _title.name = "ChatLogPanelTitle";

            // Add messagebox component
            _messageBox = _scrollablepanel.AddUIComponent<UILabel>();
            _messageBox.isVisible = true;
            _messageBox.isEnabled = true;
            _messageBox.autoSize = false;
            _messageBox.autoHeight = true;
            _messageBox.width = 470;
            _messageBox.height = 240;
            _messageBox.position = new Vector2(10, -30);
            _messageBox.textScale = 0.8f;
            _messageBox.wordWrap = true;
            _messageBox.name = "ChatLogPanelMessageBox";

            // Add scrollbar component
            _scrollbar = AddUIComponent<UIScrollbar>();
            _scrollbar.name = "Scrollbar";
            _scrollbar.width = 20f;
            _scrollbar.height = _scrollablepanel.height;
            _scrollbar.orientation = UIOrientation.Vertical;
            _scrollbar.pivot = UIPivotPoint.TopLeft;
            _scrollbar.position = new Vector2(480, -30);
            _scrollbar.minValue = 0;
            _scrollbar.value = 0;
            _scrollbar.incrementAmount = 50;
            _scrollbar.name = "ChatLogPanelScrollBar";

            // Add scrollbar background sprite component
            _trackingsprite = _scrollbar.AddUIComponent<UISlicedSprite>();
            _trackingsprite.position = new Vector2(0, 0);
            _trackingsprite.autoSize = true;
            _trackingsprite.size = _trackingsprite.parent.size;
            _trackingsprite.fillDirection = UIFillDirection.Vertical;
            _trackingsprite.spriteName = "ScrollbarTrack";
            _trackingsprite.name = "ChatLogPanelTrack";
            _scrollbar.trackObject = _trackingsprite;
            _scrollbar.trackObject.height = _scrollbar.height;

            // Add scrollbar thumb component
            _trackingthumb = _scrollbar.AddUIComponent<UISlicedSprite>();
            _trackingthumb.position = new Vector2(0, 0);
            _trackingthumb.fillDirection = UIFillDirection.Vertical;
            _trackingthumb.autoSize = true;
            _trackingthumb.width = _trackingthumb.parent.width - 8;
            _trackingthumb.spriteName = "ScrollbarThumb";
            _trackingthumb.name = "ChatLogPanelThumb";

            _scrollbar.thumbObject = _trackingthumb;
            _scrollbar.isVisible = true;
            _scrollbar.isEnabled = true;
            _scrollablepanel.verticalScrollbar = _scrollbar;

            // Add text field component (used for inputting)
            _chatText = (UITextField)AddUIComponent(typeof(UITextField));
            _chatText.width = width;
            _chatText.height = 30;
            _chatText.position = new Vector2(0, -280);
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
            _chatText.name = "ChatLogPanelChatText";

            WelcomeChatMessage();

            // Add resizable adjustments
            eventSizeChanged += (component, param) =>
            {
                _scrollablepanel.width = (width - 30);
                _scrollablepanel.height = (height - 70);
                _messageBox.width = (width - 30);
                _chatText.width = width;
                _scrollbar.height = _scrollablepanel.height;
                _trackingsprite.size = _trackingsprite.parent.size;
                _chatText.position = new Vector3(0, (-height + 30));
                _resize.position = new Vector2((width - 20), (-height + 10));
                _scrollbar.position = new Vector2((width - 20), (-30));
            };

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
                string text = _chatText.text;
                _chatText.text = string.Empty;

                // If a command, parse it
                if (text.StartsWith("/"))
                {
                    ChatCommand command = _chatCommands.Find(x => x.Command == text.TrimStart('/'));
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
                //if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.None)
                //{
                //    PrintGameMessage(MessageType.Warning, "You can only use the chat feature when hosting or connected.");
                //    return;
                //}

                // Get the player name
                string playerName = "Local";

                if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Client)
                {
                    playerName = MultiplayerManager.Instance.CurrentClient.Config.Username;
                }
                else if (MultiplayerManager.Instance.CurrentRole == MultiplayerRole.Server)
                {
                    playerName = MultiplayerManager.Instance.CurrentServer.Config.Username;
                }

                // Build and send this message
                ChatMessageCommand message = new ChatMessageCommand
                {
                    Username = playerName,
                    Message = text
                };

                Command.SendToAll(message);

                // Add the message to the chat UI
                PrintChatMessage(playerName, text);
            }
        }

        public void WelcomeChatMessage()
        {
            PrintGameMessage("Welcome to Cities: Skylines Multiplayer!");
            PrintGameMessage("Press the ~ (tilde) key to show or hide the chat.");
            PrintGameMessage("Press the tab key to switch focus to the chat.");
            PrintGameMessage("Join our discord server at: https://discord.gg/RjACPhd");
            PrintGameMessage("Type '/help' to see a list of commands and usage.");
            PrintGameMessage("Type '/support' to find out where to report bugs and get help.");
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
            PrintMessage($"<CSM> {msg}");
        }

        /// <summary>
        ///     Prints a chat message to the ChatLogPanel.
        /// </summary>
        /// <param name="username">The name of the sending user.</param>
        /// <param name="msg">The message.</param>
        public static void PrintChatMessage(string username, string msg)
        {
            PrintMessage($"<{username}> {msg}");
        }

        private static void PrintMessage(string msg)
        {
            SimulationManager.instance.m_ThreadingWrapper.QueueMainThread(() =>
            {
                UILabel messageBox = UIView.GetAView().FindUIComponent<UILabel>("ChatLogPanelMessageBox");

                // Check if the thumb is at the bottom of the scrollbar for autoscrolling
                UIScrollbar scrollBar = UIView.GetAView().FindUIComponent<UIScrollbar>("ChatLogPanelScrollBar");
                UISlicedSprite thumb = UIView.GetAView().FindUIComponent<UISlicedSprite>("ChatLogPanelThumb");
                float size = (thumb.relativePosition.y + thumb.size.y);
                bool autoScroll = Math.Abs(size - scrollBar.height) < 0.2f;

                messageBox.text += ($"{msg}\n");

                if (autoScroll)
                {
                    scrollBar.minValue = scrollBar.maxValue;
                }
            });
        }
    }
}
