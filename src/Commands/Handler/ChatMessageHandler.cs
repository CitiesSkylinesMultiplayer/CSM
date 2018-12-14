using CSM.Networking;
using CSM.Panels;

namespace CSM.Commands.Handler
{
    public class ChatMessageHandler : CommandHandler<ChatMessageCommand>
    {
        public override byte ID => CommandIds.ChatMessageCommand;

        public override void HandleOnClient(ChatMessageCommand command)
        {
            ChatLogPanel.GetDefault().AddChatMessage(command.Username, command.Message);
        }

        public override void HandleOnServer(ChatMessageCommand command, Player player)
        {
            ChatLogPanel.GetDefault().AddChatMessage(command.Username, command.Message);
        }
    }
}