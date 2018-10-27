using System;
using System.Net;
using System.Net.Sockets;

namespace CSM.Networking
{
    public class IPAddress
    {

        public static string GetLocalIPAddress()
        {
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    return endPoint.Address.ToString();
                }
            }
            catch (Exception)
            {
                return "Not found";
            }
        }

        public static string GetExternalIPAddress()
        {
            try
            {
                return new WebClient().DownloadString("https://api.ipify.org");
            }
            catch (Exception)
            {
                return "Not found";
            }
        }
    }
}
