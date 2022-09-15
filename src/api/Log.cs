using System;
using System.IO;
using System.Text;

namespace CSM.API
{
    public class Log
    {
        public static Log Instance;

        public bool LogDebug { get; set; }
        public readonly string CurrentLogFile;

        private const string Layout = "[{0}] [{1}] {2}";
        private const int KeepLogFiles = 5;
        private readonly FileStream _fileStream;

        public Log(string localApplicationData)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");

            string logDirectory = Path.Combine(localApplicationData, "multiplayer-logs/");
            Directory.CreateDirectory(logDirectory);

            CurrentLogFile = Path.Combine(logDirectory, $"log-{date}.txt");

            _fileStream = File.Open(CurrentLogFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

            // Clean up old files
            string[] logFiles = Directory.GetFiles(logDirectory, "log-*");
            for (int i = 0; i < logFiles.Length - KeepLogFiles; i++)
            {
                File.Delete(logFiles[i]);
            }
        }

        ~Log()
        {
            _fileStream.Dispose();
        }

        private void Write(string message, string level)
        {
            message = string.Format(Layout, DateTime.Now.ToString("HH:mm:ss.ffff"), level, message);
            message += Environment.NewLine;

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
