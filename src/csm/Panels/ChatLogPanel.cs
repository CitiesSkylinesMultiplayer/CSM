using ColossalFramework.UI;
using System.Collections.Generic;
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

        public override void Start()
        {
            // Activates the dragging of the window
            AddUIComponent(typeof(UIDragHandle));

            backgroundSprite = "GenericPanel";
            name = "MPChatLogPanel";
            color = new Color32(110, 110, 110, 220);

            // Grab the view for calculating width and height of game
            var view = UIView.GetAView();

            // Center this window in the game
            relativePosition = new Vector3(10.0f, view.fixedHeight - 440.0f);

            width = 470;
            height = 300;

            // Create the message box
            _messageBox = (UIListBox)AddUIComponent(typeof(UIListBox));
            _messageBox.isVisible = true;
            _messageBox.isEnabled = true;
            _messageBox.width = 450;
            _messageBox.height = 280;
            _messageBox.position = new Vector2(10, -10);

            base.Start();
        }

        public void AddMessage(string message)
        {
            // Game console
            var existingItems = new List<string>();
            existingItems.AddRange(_messageBox.items);
            existingItems.Add(message);

            _messageBox.items = existingItems.ToArray();
        }
    }
}