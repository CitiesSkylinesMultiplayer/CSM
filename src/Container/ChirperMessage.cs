using ICities;
using UnityEngine;

namespace CSM.Container
{

    public class ChirperMessage : IChirperMessage
    {
        uint IChirperMessage.senderID { get => _senderID; }
        string IChirperMessage.senderName { get => _senderName; }
        string IChirperMessage.text { get => _text; }

        public ChirperMessage(uint senderID, string senderName, string text)
        {
            _senderID = senderID;
            _senderName = senderName;
            _text = text;
        }

        private uint _senderID;
        private string _senderName;
        private string _text;

        public static ChirpPanel getChirpPanel()
        {
            if (_chirpPanel == null)
            {
                _chirpPanel = GameObject.Find("ChirperPanel").GetComponent<ChirpPanel>();
            }
            return _chirpPanel;
        }

        private static ChirpPanel _chirpPanel;
        
    }
}
