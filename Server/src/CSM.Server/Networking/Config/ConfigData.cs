using System.IO;

namespace CSM.Networking.Config
{
    public static class ConfigData
    {
        public const string ClientFile = "client-config.json";
        public const string ServerFile = "server-config.json";

        public static bool Load<T>(ref T config, string filePath) where T : new()
        {
            if (!File.Exists(filePath))
            {
                config = new T();
                return false;
            }

            try
            {
                config = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
                return true;
            }
            catch
            {
                config = new T();
                return false;
            }
        }

        public static void Save<T>(ref T config, string filePath, bool isAllowed)
        {
            if (!isAllowed)
            {
                if (!File.Exists(filePath)) { return; }
                File.Delete(filePath);
                return;
            }
            try
            {
                File.WriteAllText(filePath, Newtonsoft.Json.JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented));
            }
            catch { return; }
        }
    }
}