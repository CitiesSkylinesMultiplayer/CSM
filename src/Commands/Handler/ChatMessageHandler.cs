using CSM.Common;
using CSM.Networking;

namespace CSM.Commands.Handler
{
    public class ChatMessageHandler : CommandHandler<ChatMessageCommand>
    {
        public override byte ID => CommandIds.ChatMessageCommand;

        public override void HandleOnClient(ChatMessageCommand command)
        {
            LogManager.LogMessage($"<{command.Username}> {command.Message}");
        }

        public override void HandleOnServer(ChatMessageCommand command, Player player)
        {
            LogManager.LogMessage($"<{command.Username}> {command.Message}");
        }
    }
}