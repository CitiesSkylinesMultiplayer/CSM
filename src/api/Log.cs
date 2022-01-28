using System;
using System.IO;
using System.Linq;
using System.Text;

namespace CSM.API
{
    public class Log
    {
        private static readonly Log _instance;
        
        public static readonly Log Instance = _instance ?? (_instance = new Log());

        public bool LogDebug { get; set; }

        private const string Layout = "[{0}] [{1}] {2}\r\n";
        private const int KeepLogFiles = 5;
        private readonly FileStream _fileStream;

        private Log()
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");

            string filename = $"multiplayer-logs/log-{date}.txt";

            // Clean up old files
            _fileStream = File.Open(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            
            string[] logFiles = Directory.GetFiles("multiplayer-logs", "log-*");
            for (int i = 0; i < logFiles.Length - KeepLogFiles; i++)
            {
                File.Delete(logFiles[i]);
            }
        }

        ~Log()
        {
            _fileStream.Close();
        }
        
        private void Write(string message, string level)
        {
            message = string.Format(Layout, DateTime.Now.ToString("HH:mm:ss.ffff"), level, message);
            byte[] info = new UTF8Encoding(true).GetBytes(message);
            _fileStream.Write(info, 0, info.Length);
            _fileStream.Flush();
        }

        public static void Error(string message)
        {
            Instance.Write(message, "Error");
        }

        public static void Error(string message, Exception ex)
        {
            Instance.Write($"{message}: {ex}", "Except");
        }

        public static void Warn(string message)
        {
            Instance.Write(message, "Warn");
        }

        public static void Info(string message)
        {
            Instance.Write(message, "Info");
        }

        public static void Debug(string message)
        {
            if (Instance.LogDebug)
            {
                Instance.Write(message, "Debug");
            }
        }
    }
}
