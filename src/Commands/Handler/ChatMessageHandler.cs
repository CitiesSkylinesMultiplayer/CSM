using CSM.Panels;

namespace CSM.Commands.Handler
{
    public class ChatMessageHandler : CommandHandler<ChatMessageCommand>
    {
        public ChatMessageHandler()
        {
            TransactionCmd = false;
        }

        public override void Handle(ChatMessageCommand command)
        {
            ChatLogPanel.PrintChatMessage(command.Username, command.Message);
        }
    }
}
