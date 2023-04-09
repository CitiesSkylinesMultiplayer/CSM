namespace CSM.API
{
    public abstract class Chat
    {
        public static IChat Instance { get; set; }
        
        public enum MessageType
        {
            Normal,
            Warning,
            Error
        }
    }

    public interface IChat
    {
        void PrintGameMessage(string msg);
        void PrintGameMessage(Chat.MessageType type, string msg);
        void PrintChatMessage(string username, string msg);
        string GetCurrentUsername();
    }
}
