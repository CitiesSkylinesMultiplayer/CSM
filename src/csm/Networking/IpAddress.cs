using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using CSM.API;
using CSM.Util;
using LiteNetLib;

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
        public static string GetLocalIpAddress()
        {
            try
            {
                //Create a new socket
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    //Connect to some server to non-listening port
                    socket.Connect(CSM.Settings.ApiServer, 65530);
                    //Get the IP Address (Internal) from the IPEndPoint
                    return socket.LocalEndPoint is IPEndPoint endPoint ? endPoint.Address.ToString() : "";
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
            try
            {
                //Get the External IP address from internet
                return new CSMWebClient().DownloadString($"http://{CSM.Settings.ApiServer}/api/ip");
            }
            catch (Exception e)
            {
                //On error return "Not found"
                Log.Error("Failed to request IP: " + e.Message);
                return "Not found";
            }
        }

        public static PortState CheckPort(int port)
        {
            CSMWebClient client = new CSMWebClient();
            try
            {
                string answer = client.DownloadString($"http://{CSM.Settings.ApiServer}/api/check?port=" + port);
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

        public static IPAddress GetIpv4(string host)
        {
            return Dns.GetHostEntry(host).AddressList.FirstOrDefault(resolveAddress => resolveAddress.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}
