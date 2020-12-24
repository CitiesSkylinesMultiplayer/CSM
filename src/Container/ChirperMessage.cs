using ICities;
using System;

namespace CSM.Container
{
    public class ChirperMessage : IChirperMessage
    {
        uint IChirperMessage.senderID { get => _senderID; }
        string IChirperMessage.senderName { get => _senderName; }
        string IChirperMessage.text { get => _text; }

        private uint _senderID;
        private string _senderName;
        private string _text;

        public ChirperMessage(uint senderID, string senderName, string text)
        {
            _senderID = senderID;
            _senderName = senderName;
            _text = text;
        }
    }
}
