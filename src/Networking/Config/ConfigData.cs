using System.IO;
using UnityEngine;

namespace CSM.Networking.Config
{
    public static class ConfigData
    {
        const string clientFile = "client-config.json";
        const string serverFile = "server-config.json";

        public static void Load(ref ClientConfig config)
        {
            config = Read<ClientConfig>(clientFile);
        }

        public static void Load(ref ServerConfig config)
        {
            config = Read<ServerConfig>(serverFile);
        }

        public static void Save(ref ClientConfig config)
        {
            Write<ClientConfig>(config, clientFile);
        }

        public static void Save(ref ServerConfig config)
        {
            Write<ServerConfig>(config, serverFile);
        }

        private static T Read<T>(string path) where T : new()
        {
            if (!File.Exists(path)) { return new T(); }

            try
            {
                return JsonUtility.FromJson<T>(File.ReadAllText(path));
            }
            catch { return new T(); }
        }

        private static void Write<T>(T config, string path)
        {
            try
            {
                File.WriteAllText(path, JsonUtility.ToJson(config, true));
            }
            catch { }
        }
    }
}
