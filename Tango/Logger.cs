using System;

namespace Tango
{
    public static class Logger
    {
        public static void Info(string message)
        {
            Console.Write(message);
        }

        public static void Fatal(string message, Exception ex)
        {
            Console.Write(message + " :: " + ex.Message);
        }
    }
}
