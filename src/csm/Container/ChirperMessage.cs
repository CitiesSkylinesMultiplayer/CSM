
namespace CSM.Container
{
    /// <summary>
    /// Implementation of the IChirper Interface.
    /// This allows displaying messages to the user using the Chirper at the top of the screen.
    /// </summary>
    public class ChirperMessage : MessageBase
    {
        private string _senderName;
        private string _text;

        /// <summary>
        /// Constructs a ChirperMessage object.
        /// </summary>
        /// <param name="senderName">Sender name, this will apper as blue text above the message</param>
        /// <param name="text">The body of the message</param>
        public ChirperMessage(string senderName, string text)
        {
            _senderName = senderName;
            _text = text;
        }

        public override string GetSenderName()
        {
            return _senderName;
        }

        public override string GetText()
        {
            return _text;
        }

        public override bool IsSimilarMessage(MessageBase other)
        {
            return this.senderName == other.senderName;
        }
    }
}
