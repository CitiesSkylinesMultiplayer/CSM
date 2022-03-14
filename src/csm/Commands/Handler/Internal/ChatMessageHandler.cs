using CSM.API;
using CSM.API.Commands;
using CSM.Commands.Data.Internal;

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
            Chat.Instance.PrintChatMessage(command.Username, command.Message);
        }
    }
}
