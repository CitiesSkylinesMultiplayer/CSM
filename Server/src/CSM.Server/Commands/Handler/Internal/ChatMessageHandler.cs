using CSM.Commands.Data.Internal;
using CSM.Server.Util;

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
