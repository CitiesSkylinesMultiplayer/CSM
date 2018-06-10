using System;
using CSM.Networking;
using CSM.Networking.Config;

namespace CSM.Testing
{
    class Program
    {
        private Client _client;
        private Server _server;

        static void Main(string[] args)
        {
            // We are not running in unity
            CitiesSkylinesMultiplayer.IsUnity = false;

            // Start program
            new Program().Start(args);
        }

        private void Start(string[] args)
        {
            // Intro
            Console.WriteLine("Welcome to the Tango testing application.");
            Console.WriteLine("Start server or client (with default settings)? S/C");

            // Get option
            var option = Console.ReadKey().Key;

            if (option == ConsoleKey.S)
            {
                _server = new Server();
                _server.StartServer(new ServerConfig());

            }
            else if (option == ConsoleKey.C)
            {
                _client = new Client();
                _client.Connect(new ClientConfig("localhost", 4230, "Tango Client"));
            }
            else
            {
                Console.WriteLine("Invalid choice, closing application.");
            }
        }
    }
}
