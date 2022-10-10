using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using CSM.API;
using CSM.Util;

namespace CSM.Networking
{
    public struct PortState
    {
        public string message;
        public HttpStatusCode status;

        public PortState(string message, HttpStatusCode status)
        {
            this.message = message;
            this.status = status;
        }
    }

    public static class IpAddress
    {
        private static string _localIp;
        private static string _externalIp;

        public static string GetLocalIpAddress()
        {
            if (_localIp != null)
            {
                return _localIp;
            }

            try
            {
                //Create a new socket
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    //Connect to some server to non-listening port
                    socket.Connect("api.citiesskylinesmultiplayer.com", 65530);
                    //Get the IPEndPoint
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    //Get the IP Address (Internal) from the IPEndPoint
                    _localIp = endPoint.Address.ToString();
                    return _localIp;
                }
            }
            catch (Exception)
            {
                //On error return "Not found"
                return "Not found";
            }
        }

        public static string GetExternalIpAddress()
        {
            if (_externalIp != null)
            {
                return _externalIp;
            }

            try
            {
                //Get the External IP address from internet
                _externalIp = new CSMWebClient().DownloadString("http://api.citiesskylinesmultiplayer.com/api/ip");
                return _externalIp;
            }
            catch (Exception e)
            {
                //On error return "Not found"
                Log.Error("Failed to request IP: " + e.Message);
                return "Not found";
            }
        }

        public static string GetVPNIpAddress()
        {
            try
            {
                //Create a new socket
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    // Try to connect to random address in 25.0.0.0/8 subnet used by Hamachi
                    socket.Connect("25.0.0.0", 65530);
                    // If local address used starts with 25., Hamachi is installed and active
                    if (socket.LocalEndPoint is IPEndPoint endPoint && endPoint.Address.GetAddressBytes()[0] == 25)
                    {
                        return endPoint.Address.ToString();
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return null;
        }

        public static PortState CheckPort(int port)
        {
            CSMWebClient client = new CSMWebClient();
            try
            {
                string answer = client.DownloadString("http://api.citiesskylinesmultiplayer.com/api/check?port=" + port);
                return new PortState(answer, client.StatusCode());
            }
            catch (WebException e)
            {
                if (e.Response is HttpWebResponse response)
                {
                    Encoding encoding = response.CharacterSet != null ? Encoding.GetEncoding(response.CharacterSet) : Encoding.ASCII;
                    using (Stream stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            using (StreamReader reader = new StreamReader(stream, encoding))
                            {
                                return new PortState(reader.ReadToEnd(), response.StatusCode);
                            }
                        }
                        else
                        {
                            return new PortState(e.Message, HttpStatusCode.InternalServerError);
                        }
                    }
                }
                else
                {
                    return new PortState(e.Message, HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception e)
            {
                return new PortState(e.Message, HttpStatusCode.InternalServerError);
            }
        }
    }
}
