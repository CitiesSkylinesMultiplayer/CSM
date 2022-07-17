using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel.Channels;
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
                    //Connect to 8.8.8.8 (Google IP)
                    socket.Connect("csm-check.kaenganxt.dev", 65530);
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
                _externalIp = new CSMWebClient().DownloadString("http://csm-check.kaenganxt.dev/api/ip");
                return _externalIp;
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
                string answer = client.DownloadString("http://csm-check.kaenganxt.dev/api/check?port=" + port);
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
