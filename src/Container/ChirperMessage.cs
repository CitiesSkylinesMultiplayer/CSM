using ICities;
using UnityEngine;

namespace CSM.Container
{
    /// <summary>
    /// Implementation of the IChirper Interface.
    /// This allows displaying messages to the user using the Chirper at the top of the screen.
    /// </summary>
    public class ChirperMessage : IChirperMessage
    {
        uint IChirperMessage.senderID { get => _senderID; }
        string IChirperMessage.senderName { get => _senderName; }
        string IChirperMessage.text { get => _text; }

        private uint _senderID;
        private string _senderName;
        private string _text;
        private static ChirpPanel _chirpPanel;

        /// <summary>
        /// Constructs a ChirperMessage object.
        /// </summary>
        /// <param name="senderName">Sender name, this will apper as blue text above the message</param>
        /// <param name="text">The body of the message</param>
        /// <param name="senderID">This does not appear to be important for our use case; defaults to 0.</param>
        public ChirperMessage(string senderName, string text, uint senderID = 0)
        {
            _senderID = senderID;
            _senderName = senderName;
            _text = text;
        }

        /// <summary>
        /// Returns a reference to the ChirpPanel GameObject.
        /// This is meant to reduce expensive GameObject.Find calls by caching the result.
        /// </summary>
        /// <returns></returns>
        public static ChirpPanel getChirpPanel()
        {
            if (_chirpPanel == null)
            {
                _chirpPanel = GameObject.Find("ChirperPanel").GetComponent<ChirpPanel>();
            }
            return _chirpPanel;
        }
    }
}
