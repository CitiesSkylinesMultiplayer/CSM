using System;
using UnityEngine;

namespace CSM.Models
{
    [Serializable]
    public class JoinGameData
    {
        public string IP;
        public string Port;
        public string Username;
        public string Password;

        public JoinGameData()
        {
            IP = "localhost";
            Port = "4320";
            Username = "";
            Password = "";
        }

        public void Update(string ip, string port, string username, string password)
        {
            IP = ip;
            Port = port;
            Username = username;
            Password = password;
        }

        public string SaveToString()
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}
