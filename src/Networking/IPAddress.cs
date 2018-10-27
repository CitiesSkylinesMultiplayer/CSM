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
                //Create a new socket
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    //Connect to 8.8.8.8 (Google IP)
                    socket.Connect("8.8.8.8", 65530);
                    //Get the IPEndPoint
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    //Get the IP Address (Internal) from the IPEndPoint
                    return endPoint.Address.ToString();
                }
            }
            catch (Exception)
            {
                //On error return "Not found"
                return "Not found";
            }
        }

        public static string GetExternalIPAddress()
        {
            try
            {
                //Get the External IP (IPv4) Address from internet
                return new WebClient().DownloadString("http://api.ipify.org"); //HTTPS doesnt work
            }
            catch (Exception)
            {
                //On error return "Not found"
                return "Not found";
            }
        }
    }
}
