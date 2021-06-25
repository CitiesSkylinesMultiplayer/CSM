using CSM.API.Commands;
using CSM.Commands.Data.Internal;
using CSM.Panels;

namespace CSM.Commands.Handler.Internal
{
    public class ChatMessageHandler : CommandHandler<ChatMessageCommand>
    {
        public ChatMessageHandler()
        {
            TransactionCmd = false;
        }

        protected override void Handle(ChatMessageCommand command)
        {
            ChatLogPanel.PrintChatMessage(command.Username, command.Message);
        }
    }
}
